using Meetly_Backend.DTOs;
using Meetly_Backend.Utils;

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
        Task<ServiceResult<MeetingParticipantResponse>> ReviewMeetingAsync(int meetingId, MeetingReviewRequest request, string userId);
        Task<ServiceResult> CancelRegistrationAsync(int meetingId, string userId);
        Task<ICollection<CategoryResponse>> GetAllCategoriesAsync();
    }
}
