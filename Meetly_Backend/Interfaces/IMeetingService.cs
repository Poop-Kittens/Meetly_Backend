using Meetly_Backend.Common;
using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ICollection<BasicMeetingResponse>> GetUpcomingMeetingsAsync();
        Task<ServiceResult> CreateMeetingAsync(CreateMeetingRequestDTO dto, string hostId);
    }
}
