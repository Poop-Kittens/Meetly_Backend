using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.DTOs
{
    public record UserResponse(
        string Id,
        string UserName,
        string Email
        );
    public record UserRegister(
        [Required, MinLength(3), MaxLength(30)] string UserName,
        [Required, EmailAddress] string Email,
        [Required] string Password
);
}
