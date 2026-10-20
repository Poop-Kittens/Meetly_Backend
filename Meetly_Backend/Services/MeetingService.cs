using Meetly_Backend.Common;
using Meetly_Backend.Data;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Meetly_Backend.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly MeetlyDbContext _context;

        public MeetingService(MeetlyDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Host)
                .Include(m => m.Categories)
                .Include(m => m.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting is null)
            {
                return ServiceResult.NotFound<MeetingResponse>($"Meeting with id {id} was not found.");
            }

            var response = new MeetingResponse(
                Id: meeting.Id.ToString(),
                Title: meeting.Title,
                Description: meeting.Description,
                Location: meeting.Location,
                Date: meeting.Date,
                RemainingSlots: Math.Max(0, meeting.MaxAttendants - meeting.Participants.Count),
                Participants: meeting.Participants
                .Select(p => new MeetingParticipantResponse(
                        p.Review,
                        p.Grade,
                        p.User.UserName ?? ""))
                    .ToList(),
                Categories: meeting.Categories.Select(c => c.Title).ToList(),
                Host: new UserResponse(meeting.Host.Id, meeting.Host.UserName!, meeting.Host.Email!)
            );
            return response;
        }
    }
}
