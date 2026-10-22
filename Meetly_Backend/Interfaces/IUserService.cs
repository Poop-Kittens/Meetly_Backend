using Meetly_Backend.DTOs;
using Meetly_Backend.Utils;
using Microsoft.AspNetCore.Identity.Data;

namespace Meetly_Backend.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<string>> RegisterAsync(UserRegister request);
        Task<ServiceResult<string>> LoginAsync(LoginRequest request);
        Task<ServiceResult<string>> LogoutAsync();
        Task<ServiceResult<UserResponse>> GetCurrentUserAsync(string userId);
    }
}
