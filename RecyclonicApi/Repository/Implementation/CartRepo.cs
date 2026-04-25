using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class CartRepo : GenericRepo<Cart>, ICartRepo
    {
        public CartRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<Cart> GetCartwithCartItems(Guid userId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(ci => ci.marketplaceItem)
                .ThenInclude(m => m.ImagesUrl)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return cart;
        }
    }
}
