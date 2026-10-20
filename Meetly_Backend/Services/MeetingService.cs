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
    }
}
