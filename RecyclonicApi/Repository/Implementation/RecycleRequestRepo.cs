using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class RecycleRequestRepo : GenericRepo<RecycleRequest>, IRecycleRequestRepo
    {
        public RecycleRequestRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecycleRequest>> GetAllrequeststhatnotresponding()
        {
            return await _context.RecycleRequests
                .Include(r => r.ewasteItem)
                    .ThenInclude(e => e.ImagesUrl)
                .Where(x => x.UserResponse != false)
                .Where(x => x.Status == "Pending")
                .ToListAsync();

        }

        public async Task<RecycleRequest> GetRecycleRequestById(Guid Id)
        {
            var request = await _context.RecycleRequests
                .Include(r => r.user)
                .Include(r => r.Employee)
                .Include(r => r.ewasteItem)
                    .ThenInclude(e => e.ImagesUrl)
                .FirstOrDefaultAsync(r => r.Id == Id);
            return request;

        }

        public async Task<IEnumerable<RecycleRequest>> GetRecycleRequestsforuser(Guid Id)
        {
            var requests = await _context.RecycleRequests
                .Include(r => r.user)
                .Include(r => r.Employee)
                .Include(r => r.delivery)
                .Include(r => r.ewasteItem)
                    .ThenInclude(e => e.ImagesUrl)
                .Where(r => r.UserId == Id)
                .ToListAsync();

            return requests;
        }

        public async Task<IEnumerable<RecycleRequest>> GetRecycleRequestsforadmin(Guid Id)
        {
            var requests = await _context.RecycleRequests
                .Include(r => r.user)
                .Include(r => r.Employee)
                .Include(r => r.delivery)
                .Include(r => r.ewasteItem)
                    .ThenInclude(e => e.ImagesUrl)
                .ToListAsync();

            return requests;
        }
    }
}
