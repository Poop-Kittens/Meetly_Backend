using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync();
        Task<ICollection<BasicMeetingResponse>> SearchMeetingsAsync(MeetingSearchRequest request);
    }
}
