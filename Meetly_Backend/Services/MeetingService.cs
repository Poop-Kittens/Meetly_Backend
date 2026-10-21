using Meetly_Backend.Common;
using Meetly_Backend.Data;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Mappers;
using Meetly_Backend.Models;
using Microsoft.EntityFrameworkCore;
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
                .Where(m => m.Date > DateTime.Now)
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
                return ServiceResult.NotFound<MeetingResponse>($"Meeting with id {id} was not found.");
            }

            return meeting;
        }

        public async Task<ServiceResult> CreateMeetingAsync(CreateMeetingRequestDTO dto, string hostId)
        {
            if (dto.Date < DateTime.Now)
            {
                return ServiceResult.Invalid("Mötets datum kan inte vara i det förflutna");
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
                return ServiceResult.Invalid("En eller flera av de angivna kategorierna finns inte.");
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
            return ServiceResult.Ok();
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
                .Where(m => m.Participants.Any(p => p.UserId == userId))
                .Select(Mappings.BasicMeetingExpr)
                .ToListAsync();
        }

        public async Task<ServiceResult<MeetingResponse>> SignUpAsync(int meetingId, string userId)
        {
            // Serializable så att två som anmäler sig till sista platsen samtidigt inte båda lyckas:
            await using var tx = await _ctx.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var info = await _ctx.Meetings
                .Where(m => m.Id == meetingId)
                .Select(Mappings.SignUpInfoExpr(userId))
                .FirstOrDefaultAsync();

            if (info is null)
            {
                return ServiceResult.NotFound<MeetingResponse>($"Meeting with id {meetingId} was not found.");
            }

            if (info.Date < DateTime.UtcNow)
            {
                return ServiceResult.Invalid<MeetingResponse>("This meeting has already taken place.");
            }

            if (info.AlreadyJoined)
            {
                return ServiceResult.Conflict<MeetingResponse>("You are already signed up for this meeting.");
            }

            if (info.RemainingSlots <= 0)
            {
                return ServiceResult.Conflict<MeetingResponse>("This meeting is full, you cannot sign up.");
            }

            await _ctx.MeetingParticipants.AddAsync(new MeetingParticipant
            {
                MeetingId = meetingId,
                UserId = userId
            });

            await _ctx.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetDetailsByIdAsync(meetingId);
        }

        public async Task<ServiceResult> CancelRegistrationAsync(int meetingId, string userId)
        {
            //letar efter en anmälan som matchar, med rätt meetup och rätt användare
            var registration = await _ctx.MeetingParticipants
                .FirstOrDefaultAsync(mp => mp.MeetingId == meetingId && mp.UserId == userId);

            //om ingen anmälan hittas
            if (registration == null)
            {
                return ServiceResult.NotFound("Du har ingen anmälan till detta möte");
            }

            if (registration.Meeting.Date < DateTime.UtcNow)
            {
                return ServiceResult.Invalid("Mötet har redan ägt rum.");
            }

            _ctx.MeetingParticipants.Remove(registration);

            await _ctx.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
