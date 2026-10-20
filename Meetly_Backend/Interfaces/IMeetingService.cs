using Meetly_Backend.Common;
using Meetly_Backend.DTOs;

namespace Meetly_Backend.Interfaces
{
    public interface IMeetingService
    {
        Task<ServiceResult<MeetingResponse>> GetDetailsByIdAsync(int id);
    }
}
