using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class MarketPlaceRepo : GenericRepo<MarketplaceItem> , IMarketPlaceRepo
    {
        public MarketPlaceRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<MarketplaceItem>> GetAllinMarketPlace()
        {
            var items = await _context.MarketplaceItems
            .Include(x => x.ImagesUrl)  
            .ToListAsync();

            return items;
        }
        public async Task<ApplicationUser> userwithfavitems(string userId)
        {
            Guid guidUserId = Guid.Parse(userId);

            var userWithFavourites = await _context.Users
                .Include(u => u.FavouriteItems) 
                .FirstOrDefaultAsync(u => u.Id == guidUserId);

            return userWithFavourites;
        }

    }
}
