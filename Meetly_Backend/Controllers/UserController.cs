using Meetly_Backend.Common;
using Meetly_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Meetly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser([FromBody] RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request);
            return user.ToActionResult(this);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody]LoginRequest request)//from body innebär att det kommer från själva apianropets innehåll istället för typ URL
        {
            var user = await _userService.LoginAsync(request);
            return user.ToActionResult(this);
        }

        [Authorize]//detta för att endast inloggade personer ska kunna anropa endpointen
        [HttpPost("Logout")]
        public async Task<ActionResult<string>> Logout()
        {
            var user = await _userService.LogoutAsync();
            return user.ToActionResult(this);
        }
    }
}
