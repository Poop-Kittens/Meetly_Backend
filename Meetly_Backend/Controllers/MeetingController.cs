using Meetly_Backend.DTOs;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meetly_Backend.Controllers
{
    [Authorize]
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

        [HttpGet("search")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> Search([FromQuery] MeetingSearchRequest request)
        {
            return Ok(await _meetingService.SearchMeetingsAsync(request));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingResponse>> GetById(int id)
        {
            var result = await _meetingService.GetDetailsByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        public async Task<ActionResult<MeetingResponse>> CreateMeeting([FromBody] CreateMeetingRequest request)
        {
            //anropar tjänsten för att skapa ett möte
            var result = await _meetingService.CreateMeetingAsync(request, User.GetUserId());

            //returnerar resultatet av tjänsten som ett ActionResult
            return result.ToActionResult(this,
                m => CreatedAtAction(nameof(GetById), new { id = m.Id }, m));
        }

        [HttpPost("{id:int}/registration")]
        public async Task<ActionResult<MeetingResponse>> SignUp(int id)
        {
            var result = await _meetingService.SignUpAsync(id, User.GetUserId());
            return result.ToActionResult(this);
        }

        [HttpGet("user")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> GetUser()
        {
            return Ok(await _meetingService.GetUserMeetingsAsync(User.GetUserId()));
        }

        [HttpPut("{id:int}/review")]
        public async Task<ActionResult<MeetingParticipantResponse>> AddReview(
            int id, [FromQuery] MeetingReviewRequest request)
        {
            var result = await _meetingService.ReviewMeetingAsync(id, request, User.GetUserId());
            return result.ToActionResult(this);
        }

        //DELETE-endpoint där id är meetupets ID
        [HttpDelete("{id:int}/registration")]
        public async Task<IActionResult> CancelRegistration(int id)
        {
            //anropar Service som hittar och tar bort användarens anmälan
            var result = await _meetingService.CancelRegistrationAsync(id, User.GetUserId());

            //omvändlar resultatet frpn Service till ett HTTP-svarS
            return result.ToActionResult(this);
        }

        //PUT-endpoint för att uppdatera ett möte
        [HttpPut("{id}")]
        public async Task<ActionResult<MeetingResponse>> UpdateMeeting(int id, [FromBody] UpdateMeetingRequest dto)
        {
            //hämtar den inloggade användarens id via identity
            var hostId = _userManager.GetUserId(User);

            if (hostId == null)
            {
                return Unauthorized("Användaren finns inte.");
            }

            //anropar Service för att uppdatera mötet
            var result = await _meetingService.UpdateMeetingAsync(id, dto, hostId);

            return result.ToActionResult(this);
        }
    }
}
