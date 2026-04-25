using RecyclonicApi.Models;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface ICartRepo : IGenericRepo<Cart>
    {
        Task<Cart> GetCartwithCartItems(Guid userId);
    }
}
