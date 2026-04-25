using RecyclonicApi.Models;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface IImageRepo : IGenericRepo<Images>
    {
        Task<List<Images>> GetImagesByEwasteIdAsync(Guid ewasteId);
    }
}
