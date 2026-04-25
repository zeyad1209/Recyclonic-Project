using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Security.Claims;

namespace RecyclonicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService _authService;
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AuthController(IAuthService authService, AppDbContext context)
            //, UserManager<ApplicationUser> userManager , AppDbContext context)
        {
            _authService = authService;
            //_userManager = userManager;
            _context = context;
        }

        [HttpPost("Register_User")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.Register(registerDto);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }
        [HttpPost("VerifyUser")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyUserDto dto)
        {
            var result = await _authService.VerifyUser(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new
                {
                    Success = result.IsSuccess,
                    Message = result.Message
                });

            return Created(string.Empty,
                new
                {
                    Success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data
                });
        }
        [HttpPost("Log_in")]
        public async Task<IActionResult> Log_in([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(dto);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
                Data = result.Data,
            });
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] string Refreshtoken)
        {
            var result = await _authService.Refresh(Refreshtoken);
            
            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
                Data = result.Data,
            });
        }

        [HttpPost("Log_out")]
        [Authorize]
        public async Task<IActionResult> Log_out([FromBody] string Refreshtoken)
        {
            var result = await _authService.Log_out(Refreshtoken);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }

        [HttpPost("Log_out_All")]
        [Authorize]
        public async Task<IActionResult> Log_out_All([FromBody] string Refreshtoken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authService.Log_out_All(userIdStr);
            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }
        [HttpPost("Change_Password")]
        [Authorize]
        public async Task<IActionResult> Change_Password(Change_Password_Dto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authService.Change_Password(dto, userId);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }

        [HttpPost("Forget_Password")]
        public async Task<IActionResult> Forget_Password(string email)
        {
            var result = await _authService.Forget_Password(email);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }

        [HttpPost("Reset_Password")]
        public async Task<IActionResult> Reset_Password(Reset_Password_Dto dto)
        {
            var result = await _authService.Reset_Password(dto);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }
        [HttpPost("Edit_or_complete_info")]
        [Authorize]
        public async Task<IActionResult> Editorcompletedata(editorcompleteprofiledata dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authService.Editorcompletedata(userId,dto);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message
            });
        }
        [HttpGet("My-Profile")]
        [Authorize]
        public async Task<IActionResult> Myprofile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authService.Getmyprofile(userId);

            return StatusCode(result.StatusCode, new
            {
                Success = result.IsSuccess,
                Message = result.Message,
                data = result.Data
            });
        }
        #region comment delete all
        //[HttpDelete("delete-everything")]
        //public async Task<IActionResult> DeleteEverything()
        //{
        //    // 1 — StatusTracking
        //    var statuses = _context.StatusTrackings.ToList();
        //    _context.StatusTrackings.RemoveRange(statuses);
        //    await _context.SaveChangesAsync();

        //    // 2 — Deliveries
        //    var deliveries = _context.Deliveries.ToList();
        //    _context.Deliveries.RemoveRange(deliveries);
        //    await _context.SaveChangesAsync();

        //    // 3 — RecycleRequests
        //    var requests = _context.RecycleRequests.ToList();
        //    _context.RecycleRequests.RemoveRange(requests);
        //    await _context.SaveChangesAsync();

        //    // 4 — EwasteItems
        //    var ewaste = _context.EwasteItems.ToList();
        //    _context.EwasteItems.RemoveRange(ewaste);
        //    await _context.SaveChangesAsync();

        //    // 5 — Images
        //    var images = _context.Images.ToList();
        //    _context.Images.RemoveRange(images);
        //    await _context.SaveChangesAsync();


        //    // 🟦 6 — Delete Identity Users
        //    var users = _userManager.Users.ToList();
        //    foreach (var user in users)
        //    {
        //        await _userManager.DeleteAsync(user);
        //    }

        //    return Ok("All data deleted successfully");
        //}
        #endregion

        #region comment fix users carts

        //    [HttpPost("fix-users-carts")]
        //    public async Task<IActionResult> CreateCartForUsersWithoutCart()
        //    {
        //        // 1️⃣ Get all user ids
        //        var usersIds = await _context.Users
        //            .Select(u => u.Id)
        //            .ToListAsync();

        //        // 2️⃣ Get user ids that already have carts
        //        var usersWithCartsIds = _context.Carts
        //.Select(c => c.UserId)
        //.ToList()   // جلب البيانات أولاً
        //.ToHashSet();  // بعدين تحويلها لـ HashSet


        //        // 3️⃣ Users without carts
        //        var usersWithoutCarts = usersIds
        //            .Where(uid => !usersWithCartsIds.Contains(uid))
        //            .ToList();

        //        if (!usersWithoutCarts.Any())
        //            return Ok("All users already have carts");

        //        // 4️⃣ Create carts
        //        var carts = usersWithoutCarts.Select(uid => new Cart
        //        {
        //            UserId = uid,
        //            CreatedAt = DateTime.Now,
        //            TotalAmount = 0,
        //            DeliveryFees = 0
        //        });

        //        await _context.Carts.AddRangeAsync(carts);
        //        await _context.SaveChangesAsync();

        //        return Ok($"{usersWithoutCarts.Count} carts created successfully");
        //    }
        #endregion


    }
}
