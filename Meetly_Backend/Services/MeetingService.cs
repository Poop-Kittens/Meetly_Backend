using Meetly_Backend.Common;
using Meetly_Backend.Data;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Mappers;
using Meetly_Backend.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
