using Meetly_Backend.Common;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Meetly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        private readonly UserManager<User> _userManager;

        public MeetingController(IMeetingService meetingService, UserManager<User> userManager)
        {
            _meetingService = meetingService;
            _userManager = userManager;
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> Upcoming()
        {
            return Ok(await _meetingService.GetUpcomingMeetingsAsync());
        }

        [HttpGet("search")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> Search([FromQuery] MeetingSearchRequest request)
        {
            return Ok(await _meetingService.SearchMeetingsAsync(request));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _meetingService.GetDetailsByIdAsync(id);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingRequestDTO dto)
        {
            //hämtar användarens ID
            var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(hostId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            //anropar tjänsten för att skapa ett möte
            var result = await _meetingService.CreateMeetingAsync(dto, hostId);

            //returnerar resultatet av tjänsten som ett ActionResult
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPost("{id}/SignUp")]
        public async Task<IActionResult> SignUp(int id)
        {
            var userId = _userManager.GetUserId(User); // Inbyggd metod i Microsoft Identity
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _meetingService.SignUpAsync(id, userId);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> GetUser(string userId)
        {
            return Ok(await _meetingService.GetUserMeetingsAsync(userId));
        }
    }
}
