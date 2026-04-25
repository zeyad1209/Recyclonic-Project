using RecyclonicApi.Models;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface IMarketPlaceRepo : IGenericRepo<MarketplaceItem>
    {
        Task<ICollection<MarketplaceItem>> GetAllinMarketPlace();
        Task<ApplicationUser> userwithfavitems(string userId);
    }
}
