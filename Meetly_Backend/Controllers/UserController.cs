using Meetly_Backend.Common;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Services;
using Microsoft.AspNetCore.Http;
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
            _userService=userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request);
           
            return user.ToActionResult(this);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Logon([FromBody]LoginRequest request)//from body innebär att det kommer från själva apianropets innehåll istället för typ URL
        {
            var user = await _userService.LogonAsync(request);
           
            return user.ToActionResult(this);
        }
    }
}
