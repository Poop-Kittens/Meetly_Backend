using Meetly_Backend.Common;
using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync();
        Task<ICollection<BasicMeetingResponse>> SearchMeetingsAsync(MeetingSearchRequest request);
        Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id);
        Task<ServiceResult> CreateMeetingAsync(CreateMeetingRequestDTO dto, string hostId);
        Task<ServiceResult<MeetingResponse>> SignUpAsync(int meetingId, string userId);
        Task<ICollection<BasicMeetingResponse>> GetUserMeetingsAsync(string userId);
    }
}
