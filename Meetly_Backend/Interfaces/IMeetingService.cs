using Meetly_Backend.Common;
using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync();
        Task<ICollection<BasicMeetingResponse>> SearchMeetingsAsync(MeetingSearchRequest request);
        Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id);
        Task<ServiceResult<MeetingResponse>> CreateMeetingAsync(CreateMeetingRequest dto, string hostId);
        Task<ServiceResult<MeetingResponse>> SignUpAsync(int meetingId, string userId);
        Task<ICollection<BasicMeetingResponse>> GetUserMeetingsAsync(string userId);
        Task<ServiceResult<MeetingParticipantResponse>> ReviewMeetingAsync(MeetingReviewRequest request, string userId);
        Task<ServiceResult> CancelRegistrationAsync(int meetingId, string userId);
        Task<ServiceResult<MeetingResponse>> UpdateMeetingAsync(int meetingId, UpdateMeetingRequest dto, string hostId);
    }
}
