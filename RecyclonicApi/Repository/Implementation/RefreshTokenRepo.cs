using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;

namespace RecyclonicApi.Repository.Implementation
{
    public class RefreshTokenRepo : GenericRepo<RefreshToken> ,IRefreshTokenRepo
    {
        public RefreshTokenRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ValidGeneratetoken(string token)
        {
            var refreshtoken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            return refreshtoken == null;
        }
        public async Task<RefreshToken> GetTokenfromtokenstring(string token)
        {
            var Token = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
            return Token;
        }
        public async Task<ApplicationUser> GetUserfromRefreshToken(string token)
        {
            var Token = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);

            return Token.User;
        }
        public void RemoveAllRefreshTokensForUser(ICollection<RefreshToken> refreshTokens)
        {
            if (refreshTokens == null || !refreshTokens.Any())
                return;

            foreach (var token in refreshTokens)
                token.IsRevoked = true;

            _context.RefreshTokens.UpdateRange(refreshTokens);
        }
    }
}
