namespace Meetly_Backend.DTOs
{
    public record UserResponse(string Id, string UserName, string Email);

    public record UserRegister(string Email, string Password);
}
