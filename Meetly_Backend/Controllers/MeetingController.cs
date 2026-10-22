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
    [Authorize]
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

        [HttpPost("Create")]
        public async Task<ActionResult<MeetingResponse>> CreateMeeting([FromBody] CreateMeetingRequest dto)
        {
            //hämtar användarens ID
            var hostId = _userManager.GetUserId(User);

            //anropar tjänsten för att skapa ett möte
            var result = await _meetingService.CreateMeetingAsync(dto, hostId!);

            //returnerar resultatet av tjänsten som ett ActionResult
            return result.ToActionResult(this);
        }

        [HttpPost("{id}/SignUp")]
        public async Task<IActionResult> SignUp(int id)
        {
            var userId = _userManager.GetUserId(User); // Inbyggd metod i Microsoft Identity
            
            var result = await _meetingService.SignUpAsync(id, userId!);
            return Ok(result.Data);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ICollection<BasicMeetingResponse>>> GetUser(string userId)
        {
            return Ok(await _meetingService.GetUserMeetingsAsync(userId));
        }

        [HttpPost("review")]
        public async Task<ActionResult<MeetingParticipantResponse>> AddReview(
            [FromQuery] MeetingReviewRequest request)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _meetingService.ReviewMeetingAsync(request, userId!);
            return result.ToActionResult(this);
        }

        //DELETE-endpoint där id är meetupets ID
        [HttpDelete("Registration/{id}")]
        public async Task<IActionResult> CancelRegistration(int id)
        {
            //hämtar den inloggade användarens id
            var userId = _userManager.GetUserId(User);

            //anropar Service som hittar och tar bort användarens anmälan
            var result = await _meetingService.CancelRegistrationAsync(id, userId!);

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
