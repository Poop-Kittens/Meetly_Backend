using Meetly_Backend.Common;
using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Meetly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> Upcoming()
        {
            return Ok(await _meetingService.GetUpcomingMeetingsAsync());
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
    }
}
