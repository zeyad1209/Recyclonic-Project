using RecyclonicApi.Models;
using RecyclonicApi.Repository.Implementation;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface IRecycleRequestRepo : IGenericRepo<RecycleRequest>
    {
        Task<IEnumerable<RecycleRequest>> GetAllrequeststhatnotresponding();
        Task<RecycleRequest> GetRecycleRequestById(Guid Id);
        Task<IEnumerable<RecycleRequest>> GetRecycleRequestsforuser(Guid Id);
        Task<IEnumerable<RecycleRequest>> GetRecycleRequestsforadmin(Guid Id);
    }
}
