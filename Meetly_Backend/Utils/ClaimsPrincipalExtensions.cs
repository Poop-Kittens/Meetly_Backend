using System.Security.Claims;

namespace Meetly_Backend.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Den autentiserade användaren saknar id-anspråk.");
    }
}
