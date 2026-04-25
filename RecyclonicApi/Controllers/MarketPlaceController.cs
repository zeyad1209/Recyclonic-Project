using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Security.Claims;

namespace RecyclonicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketPlaceController : ControllerBase
    {
        readonly IMarketPlaceService _marketPlaceService;
        readonly AppDbContext _context;

        public MarketPlaceController(IMarketPlaceService marketPlaceService, AppDbContext context)
        {
            _marketPlaceService = marketPlaceService;
            _context = context;
        }

        [HttpPost("AddProductinMarketPlace")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Addproduct([FromForm] AddMarketplaceItemDto dto)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _marketPlaceService.Add_Product(userid, dto);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _marketPlaceService.GetAllProducts(userid);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPatch("PressFavourite")]
        [Authorize]
        public async Task<IActionResult> PressFavourite(Guid ItemId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _marketPlaceService.Press_Favourite(userid , ItemId);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message,
            });
        }
        [HttpPost("Addtocart/{ItemId}")]
        [Authorize]
        public async Task<IActionResult> Addtocart([FromRoute]Guid ItemId, [FromQuery] int Quantity)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _marketPlaceService.AddToCart(userid , ItemId, Quantity);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message,
            });
        }
        [HttpGet("GetMycart")]
        [Authorize]
        public async Task<IActionResult> GetMyCart()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _marketPlaceService.GetMyCart(userid);
            return StatusCode(result.StatusCode, new
            {
                Succes = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }
        [HttpGet()]
        public async Task<IActionResult> Co()
        {
            var v = await _context.CartItems.ToListAsync();
            return Ok(v.Count);
        }
    }
}
