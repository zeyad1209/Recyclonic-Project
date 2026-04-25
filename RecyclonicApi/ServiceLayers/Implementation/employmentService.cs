using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecyclonicApi.Data;
using RecyclonicApi.HelperServices;
using RecyclonicApi.Models;
using RecyclonicApi.Models.Domain;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.Repository.Interfaces;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Net;
using static System.Net.WebRequestMethods;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class employmentService : IemploymentService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly EmailService _emailService;
        readonly IMapper _mapper;
        readonly AppDbContext _context;
        readonly IInvitationRepo _invitationRepo;
        readonly IHelperService _helperservice;

        public employmentService(UserManager<ApplicationUser> userManager,
            EmailService emailService,
            IMapper mapper,
            AppDbContext context,
            IInvitationRepo invitationRepo,
            IHelperService helperservice)
        {
            _userManager = userManager;
            _emailService = emailService;
            _mapper = mapper;
            _context = context;
            _invitationRepo = invitationRepo;
            _helperservice = helperservice;
        }

        public async Task<ServiceResult> employmentUser(string AdminId, employmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(AdminId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var Admin = await _userManager.FindByIdAsync(AdminId);
            if (Admin == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            dto.ExpiredDate ??= DateTime.Now.AddMonths(3);

            var user = await _userManager.FindByEmailAsync(dto.Email);

            var invitation = new Invitation
            {
                Role = dto.Role,
                Expired = false,
                ExpiredDate = dto.ExpiredDate,
                CreateddDate = DateTime.Now,
                Email = dto.Email,
                AdminId = Admin.Id,
                Admin = Admin,
            };

            await _context.Invitations.AddAsync(invitation);
            await _context.SaveChangesAsync();

            string emailBody = $@"
        <html>
        <body>
            <h2>Invitation from Recyclonic Admin</h2>
            <p>
                You are invited to be a/an <strong>{dto.Role}</strong> in the <strong>Recyclonic App</strong>.<br/>
                Invitation sent by: <strong>{Admin.FirstName} {Admin.LastName}</strong>.<br/>
                Expired Date: <strong>{dto.ExpiredDate:dd/MM/yyyy}</strong>.
            </p>
            <p>
                {(user == null
                            ? "Go To SignUp Page: <a href='https://recyclonic1.web.app/register'>SignUp</a>"
                            : "Go To Login Page: <a href='https://recyclonic1.web.app/login'>Login</a>")}
            </p>
        </body>
        </html>
    ";

            await _emailService.SendEmailAsync(dto.Email, "Invitation From Recyclonic App", emailBody);

            return ServiceResult.Ok(null, "The invitation has been sent", HttpStatusCode.OK);
        }


        public async Task<ServiceResult> GetMyInvitations(string UserId)
        {
            if(string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var invitations = await _invitationRepo.GetmyInvitationsAsync(user.Email);
            if (invitations == null || !invitations.Any())
                return ServiceResult.Ok(null, "No Invitations for you", HttpStatusCode.NotFound);

            var invitationsdto = _mapper.Map<IEnumerable<InvitationResponseDto>>(invitations);
            return ServiceResult.Ok(invitationsdto , "Your Invitations" , HttpStatusCode.OK);
        }

        public async Task<ServiceResult> RespondInvitation(string userId, RespondInvitationDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("You must log in", HttpStatusCode.Unauthorized);

            var invitation = await _invitationRepo.GetInvitationbyIdAsync(dto.InvitaionId);
            if (invitation == null)
                return ServiceResult.Fail("No invitation with this Id", HttpStatusCode.NotFound);

            var admin = invitation.Admin;

            if (admin == null)
                return ServiceResult.Fail("Admin who sent this invitation no longer exists", HttpStatusCode.BadRequest);

            var highestRole = await _helperservice.GetHighestRole(admin);
            if (highestRole != "Admin")
                return ServiceResult.Fail("This admin who invited you is no longer valid", HttpStatusCode.BadRequest);

            if (invitation.Expired || invitation.ExpiredDate <= DateTime.Now)
                return ServiceResult.Fail("This invitation is expired", HttpStatusCode.BadRequest);

            if (!dto.Accepted)
            {
                invitation.status = false;
                invitation.Expired = true;

                _invitationRepo.UpdateAsync(invitation);
                await _invitationRepo.Save();

                return ServiceResult.Ok(null, "The invitation was rejected successfully", HttpStatusCode.OK);
            }

            invitation.status = true;
            invitation.Expired = true;

            await _helperservice.changeRole(user, invitation.Role);
            user.role = invitation.Role;
            await _userManager.UpdateAsync(user);

            _invitationRepo.UpdateAsync(invitation);
            await _invitationRepo.Save();

            return ServiceResult.Ok(null, "The invitation was accepted successfully", HttpStatusCode.OK);
        }

    }
}
