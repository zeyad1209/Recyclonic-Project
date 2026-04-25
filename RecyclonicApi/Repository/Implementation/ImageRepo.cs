using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class ImageRepo : GenericRepo<Images>, IImageRepo
    {
        public ImageRepo(AppDbContext context) 
            : base(context)
        {
        }

        public async Task<List<Images>> GetImagesByEwasteIdAsync(Guid ewasteId)
        {
            var result = await _context.Images.Where(i => i.EwasteitemId == ewasteId).ToListAsync();
            return result;
        }
    }
}
