using Meetly_Backend.Common;
using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync();
        Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id);
        Task<ServiceResult> CreateMeetingAsync(CreateMeetingRequestDTO dto, string hostId);
        Task<ServiceResult> CancelRegistrationAsync(int meetingId, string userId);
    }
}
