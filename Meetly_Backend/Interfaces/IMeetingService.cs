using Meetly_Backend.DTOs;
using Meetly_Backend.Common;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ServiceResult> CreateMeetingAsync(CreateMeetingRequestDTO dto, string hostId);
    }
}
