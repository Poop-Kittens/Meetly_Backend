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

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request);
            if (user.Success)
            {
            return Ok(user.Data);

            }
            return user.ToActionResult(this);
        }
    }
}
