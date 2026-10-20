using Meetly_Backend.Common;
using Meetly_Backend.Data;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Mappers;
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
    }
}
