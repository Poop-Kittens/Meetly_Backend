using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _meetingService.GetDetailsByIdAsync(id);
            return Ok(result.Data);
        }
    }
}
