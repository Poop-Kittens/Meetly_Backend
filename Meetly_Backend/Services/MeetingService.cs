using Meetly_Backend.Data;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Mappers;
using Meetly_Backend.Models;
using Meetly_Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;

namespace Meetly_Backend.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly MeetlyDbContext _ctx;

        public MeetingService(MeetlyDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync()
        {
            return await _ctx.Meetings
                .AsNoTracking()
                .Where(m => m.Date > DateTime.Now)
                .OrderBy(m => m.Date)
                .Select(Mappings.BasicMeetingExpr)
                .ToListAsync();
        }

        public async Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id)
        {
            var meeting = await _ctx.Meetings
                 .AsNoTracking()
                 .Where(m => m.Id == id)
                 .Select(Mappings.MeetingExpr)
                 .FirstOrDefaultAsync();

            if (meeting is null)
            {
                return ServiceResult.NotFound<MeetingResponse>($"Mötet med id {id} hittades inte.");
            }

            return meeting;
        }

        public async Task<ServiceResult<MeetingResponse>> CreateMeetingAsync(CreateMeetingRequest dto, string hostId)
        {
            if (dto.Date < DateTime.Now)
            {
                return ServiceResult.Invalid<MeetingResponse>("Mötets datum kan inte vara i det förflutna");
            }

            //går till kategorierna till databasen
            //filtrerar fram rätt kategori
            //kollar att ID för kategorin finns
            //hämtar kategorierna som en lista
            var categories = await _ctx.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            //Distinct() räknar varje kategori-ID en gång
            //Kollar att alla valda kategorier finns i databasen, annars returneras ett felmeddelande
            if (categories.Count != dto.CategoryIds.Distinct().Count())
            {
                return ServiceResult.Invalid<MeetingResponse>("En eller flera av de angivna kategorierna finns inte.");
            }

            //omvandlar informationen från DTO:n till ett Meeting-objekt
            var meeting = new Meeting
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Date = dto.Date,
                MaxAttendants = dto.MaxAttendants,
                HostId = hostId,
                Categories = categories
            };

            //lägger till ett nytt möte i databasen
            _ctx.Meetings.Add(meeting);

            //sparar ändringarna i databasen
            await _ctx.SaveChangesAsync();

            //returnerar ett ServiceResult som visar att mötet skapades korrekt
            return await GetDetailsByIdAsync(meeting.Id);
        }

        public async Task<ICollection<BasicMeetingResponse>> SearchMeetingsAsync(MeetingSearchRequest request)
        {
            IQueryable<Meeting> query = _ctx.Meetings.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(m =>
                    m.Title.Contains(term) ||
                    (m.Description != null && m.Description.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var location = request.Location.Trim();
                query = query.Where(m => m.Location.Contains(location));
            }

            if (request.Date is DateOnly date)
            {
                // Compare a day range so the time-of-day part of DateTime doesn't break equality
                var start = date.ToDateTime(TimeOnly.MinValue);
                var end = start.AddDays(1);
                query = query.Where(m => m.Date >= start && m.Date < end);
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var category = request.Category.Trim();
                query = query.Where(m => m.Categories.Any(c => c.Title == category));
            }

            return await query
                .OrderBy(m => m.Date)
                .Select(Mappings.BasicMeetingExpr)
                .ToListAsync();
        }

        public async Task<ICollection<BasicMeetingResponse>> GetUserMeetingsAsync(string userId)
        {
            return await _ctx.Meetings
                .AsNoTracking()
                .Where(m => m.Participants.Any(p => p.UserId == userId) || m.HostId == userId)
                .Select(Mappings.BasicMeetingExpr)
                .ToListAsync();
        }

        public async Task<ServiceResult<MeetingParticipantResponse>> ReviewMeetingAsync(
            int meetingId, MeetingReviewRequest request, string userId)
        {
            var participant = await _ctx.MeetingParticipants
                .Include(p => p.Meeting)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.MeetingId == meetingId);

            if (participant == null)
            {
                return ServiceResult.NotFound<MeetingParticipantResponse>(
                    "Du har inte deltagit i det här mötet.");
            }

            if (participant.Meeting.Date > DateTime.Now)
            {
                return ServiceResult.Invalid<MeetingParticipantResponse>(
                    "Du kan inte recensera ett möte som inte har ägt rum än.");
            }

            participant.Review = request.Review;
            participant.Grade = request.Grade;

            await _ctx.SaveChangesAsync();

            return ServiceResult.Ok(participant.ToResponse());
        }

        public async Task<ServiceResult<MeetingResponse>> SignUpAsync(int meetingId, string userId)
        {
            var info = await _ctx.Meetings
                .AsNoTracking()
                .Where(m => m.Id == meetingId)
                .Select(Mappings.SignUpInfoExpr(userId))
                .FirstOrDefaultAsync();

            if (info is null)
            {
                return ServiceResult.NotFound<MeetingResponse>($"Mötet med id {meetingId} hittades inte.");
            }

            if (info.Date < DateTime.Now)
            {
                return ServiceResult.Invalid<MeetingResponse>("Mötets datum kan inte vara i det förflutna");
            }

            if (info.IsHost)
            {
                return ServiceResult.Invalid<MeetingResponse>("Du är redan värd för det här mötet.");
            }

            if (info.AlreadyJoined)
            {
                return ServiceResult.Conflict<MeetingResponse>("Du har redan anmält dig till det här mötet.");
            }

            if (info.RemainingSlots <= 0)
            {
                return ServiceResult.Conflict<MeetingResponse>("Det här mötet är redan fullbokat.");
            }

            _ctx.MeetingParticipants.Add(new MeetingParticipant
            {
                MeetingId = meetingId,
                UserId = userId
            });

            await _ctx.SaveChangesAsync();

            return await GetDetailsByIdAsync(meetingId);
        }

        public async Task<ServiceResult> CancelRegistrationAsync(int meetingId, string userId)
        {
            //letar efter en anmälan som matchar, med rätt meetup och rätt användare
            var registration = await _ctx.MeetingParticipants
                .Where(mp => mp.Meeting.Date > DateTime.Now)
                .FirstOrDefaultAsync(mp => mp.MeetingId == meetingId && mp.UserId == userId);

            //om ingen anmälan hittas
            if (registration == null)
            {
                return ServiceResult.NotFound("Du har ingen anmälan till detta möte");
            }

            _ctx.MeetingParticipants.Remove(registration);

            await _ctx.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ICollection<CategoryResponse>> GetAllCategoriesAsync()
        {
            return await _ctx.Categories
                .AsNoTracking()
                .Select(Mappings.CategoryExpr)
                .ToListAsync();
        }
    }
}
