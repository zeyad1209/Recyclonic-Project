using RecyclonicApi.HelperServices;
using RecyclonicApi.Models.DTOs;

namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IMarketPlaceService
    {
        Task<ServiceResult> Add_Product(string userId, AddMarketplaceItemDto dto);
        Task<ServiceResult> GetAllProducts(string? userId);
        Task<ServiceResult> Press_Favourite(string userId, Guid ItemId);
        Task<ServiceResult> AddToCart(string userId, Guid itemId, int quantity = 1);
        Task<ServiceResult> GetMyCart(string userId);
    }
}
