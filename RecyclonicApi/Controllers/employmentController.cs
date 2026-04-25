using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Security.Claims;

namespace RecyclonicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class employmentController : ControllerBase
    {
        readonly IemploymentService _employmentService;

        public employmentController(IemploymentService employmentService)
        {
            _employmentService = employmentService;
        }

        [HttpPost("Invite_User")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InviteUser(employmentDto dto)
        {
            var AdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _employmentService.employmentUser(AdminId, dto);
            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
            });
        }
        [HttpGet("Get_My_Invitations")]
        [Authorize]
        public async Task<IActionResult> Getmyinvitations()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _employmentService.GetMyInvitations(UserId);
            if(!result.IsSuccess)
                return StatusCode(result.StatusCode, new
                {
                    Success = result.IsSuccess,
                    Message = result.Message,
                });
            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
                data = result.Data
            });
        }
        [HttpPost("Respond_Invitation")]
        [Authorize]
        public async Task<IActionResult> Respond_Invitation(RespondInvitationDto dto)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _employmentService.RespondInvitation(UserId, dto);
            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
            });
        }
    }
}
