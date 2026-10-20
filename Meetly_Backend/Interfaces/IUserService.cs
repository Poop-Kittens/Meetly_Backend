using Meetly_Backend.Common;
using Microsoft.AspNetCore.Identity.Data;

namespace Meetly_Backend.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterRequest request);

        Task<ServiceResult<string>> LogonAsync(LoginRequest request);

        Task<ServiceResult<string>> LogoutAsync();
    }
}
