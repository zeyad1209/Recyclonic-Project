using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models.Domain;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class InvitationRepo : GenericRepo<Invitation>, IInvitationRepo
    {
        public InvitationRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Invitation>>GetmyInvitationsAsync(string Email)
        {
            var result = await _context.Invitations
                .Include(inv => inv.Admin)
                .Where(inv => inv.Email == Email)
                .ToListAsync();

            return result;
        }
        public async Task<Invitation> GetInvitationbyIdAsync(Guid Id)
        {
            var result = await _context.Invitations
                .Include(inv => inv.Admin)
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }
    }
}
