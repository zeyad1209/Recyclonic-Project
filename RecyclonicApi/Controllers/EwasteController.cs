using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class EwasteController : ControllerBase
    {
        readonly IEwasteService _ewasteService;
        readonly AppDbContext _context;
        public EwasteController(
            IEwasteService ewasteService,
            AppDbContext context)
        {
            _ewasteService = ewasteService;
            _context = context;
        }

        [HttpPost("Recycle E_waste")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> RecycleEwaste([FromForm] RecycleEwastePostDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.RecycleEwaste(userId, dto);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message
            });
        }
        [HttpGet]
        public IActionResult TestEwaste()
        {
            var result2 = _context.RecycleRequests
                .Include(r => r.ewasteItem)
                .Select(r => new
                {
                    r.Id,
                    r.Status,
                    r.OfferedPrice,
                    r.UserResponse,
                    r.CreatedAt,
                    r.Address,
                    r.EwasteItemId,
                    EwasteItem = new
                    {
                        r.ewasteItem.Id,
                        r.ewasteItem.Condition,
                        r.ewasteItem.Description,
                        r.ewasteItem.SubmissionDate,
                        r.ewasteItem.weight,
                    },
                    r.UserId
                }).ToList();

            return Ok(new
            {
                result2
            });
        }
        [HttpGet("GetAllRecycleRequeststhatneedtorespond")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetAllRequeststhatneedtorespond()
        {
            var result = await _ewasteService.GetRecycleWastestorespondit();
           
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }
        //[HttpDelete("Delete All Ewaste Items")]
        //public async Task<IActionResult> DeleteAllEwasteItems()
        //{
        //    var ewasteItems = await _context.EwasteItems.ToListAsync();
        //    var recycleRequests = await _context.RecycleRequests.ToListAsync();
        //    _context.EwasteItems.RemoveRange(ewasteItems);
        //    _context.RecycleRequests.RemoveRange(recycleRequests);
        //    await _context.SaveChangesAsync();
        //    return Ok("All Ewaste Items Deleted");
        //}
        [HttpPut("RespondRequest")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> RespondRequest(Guid RequestId , string status , decimal? price)
        {
            var EmployeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.RespondRecycleRequest(EmployeeId, status, RequestId, price);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpPut("UserRespondRequest")]
        [Authorize]
        public async Task<IActionResult> UserRespondRequest(Guid RequestId, bool accept)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.Userresponsetherequest(userId, RequestId, accept);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpGet("User_requests")]
        [Authorize]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.Getallrequestforuser(userId);
            
            // Calculate total revenue from delivered/completed orders for this user
            var totalRevenue = await _context.RecycleRequests
                .Where(r => r.UserId == Guid.Parse(userId) && r.IsDelivered)
                .SumAsync(r => r.AmountPaidToUser ?? 0);

            return StatusCode(result.StatusCode, new
            {
                Message = result.Message,
                Success = result.IsSuccess,
                Data = result.Data,
                TotalRevenue = totalRevenue
            });
        }

        [HttpPut("UpdateDeliveryStatus/{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> UpdateDeliveryStatus(Guid id, [FromBody] UpdateDeliveryStatusDto dto)
        {
            var request = await _context.RecycleRequests
                .Include(r => r.delivery)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound(new { Success = false, message = "Request not found" });

            request.IsDelivered = dto.IsDelivered;
            
            if (dto.IsDelivered)
            {
                request.AmountPaidToUser = dto.AmountPaidToUser ?? request.OfferedPrice ?? 0;
                request.AmountReceivedFromRecycler = dto.AmountReceivedFromRecycler ?? 0;

                if (request.delivery != null)
                {
                    request.delivery.Status = "Delivered";
                    var hasDelivered = await _context.StatusTrackings
                        .AnyAsync(st => st.DeliveryId == request.delivery.Id && st.Status == "Delivered");
                    if (!hasDelivered)
                    {
                        var newstatus = new StatusTraking()
                        {
                            Id = Guid.NewGuid(),
                            Status = "Delivered",
                            Dateofstatus = DateTime.Now,
                            DeliveryId = request.delivery.Id
                        };
                        await _context.StatusTrackings.AddAsync(newstatus);
                    }
                }
            }
            else
            {
                request.AmountPaidToUser = null;
                request.AmountReceivedFromRecycler = null;

                if (request.delivery != null)
                {
                    request.delivery.Status = "Pending";
                    var deliveredStatus = await _context.StatusTrackings
                        .FirstOrDefaultAsync(st => st.DeliveryId == request.delivery.Id && st.Status == "Delivered");
                    if (deliveredStatus != null)
                    {
                        _context.StatusTrackings.Remove(deliveredStatus);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Success = true, message = "Delivery status updated successfully" });
        }

        [HttpPut("Cancel-request")]
        [Authorize]
        public async Task<IActionResult> Cancel_request(Guid RequestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.Cancel_Request(userId , RequestId);
            return StatusCode(result.StatusCode, new
            {
                Message = result.Message,
                Success = result.IsSuccess,
                Data = result.Data,
            });
        }

        [HttpGet("GetAllRequests")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetAllRequests()
        {
            var AdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ewasteService.Getallrequestforadmin(AdminId);
            return StatusCode(result.StatusCode, new
            {
                Message = result.Message,
                Success = result.IsSuccess,
                Data = result.Data,
            });
        }
        [HttpGet("GetUserRequestsForAdmin/{userId}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetUserRequestsForAdmin(string userId)
        {
            var result = await _ewasteService.GetUserRequestsForAdmin(userId);
            return StatusCode(result.StatusCode, new
            {
                Message = result.Message,
                Success = result.IsSuccess,
                Data = result.Data,
            });
        }
        //[HttpPost("Addtype")]
        //public async Task<IActionResult> Addtype(string name)
        //{
        //    await _context.EwasteItemTypes.AddAsync(new EwasteItemType
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = name
        //    });
        //    await _context.SaveChangesAsync();
        //    return Ok("Type Added");
        //}
        //[HttpPost("AddBrand")]
        //public async Task<IActionResult> AddBrand(string name)
        //{
        //    await _context.EwasteItemBrands.AddAsync(new EwasteItemBrand
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = name
        //    });
        //    await _context.SaveChangesAsync();
        //    return Ok("Brand Added");
        //}
        //[HttpGet("GetAllTypesandBrands")]
        //public async Task<IActionResult> Getall()
        //{
        //    var rtypes = await _context.EwasteItemTypes.ToListAsync();
        //    var rbrands = await _context.EwasteItemBrands.ToListAsync();
        //    return Ok(new
        //    {
        //        Types = rtypes,
        //        Brands = rbrands
        //    });
        //}

    }
}
