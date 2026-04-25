using RecyclonicApi.Models;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface IRefreshTokenRepo : IGenericRepo<RefreshToken>
    {
        Task<bool> ValidGeneratetoken(string token);
        Task<RefreshToken> GetTokenfromtokenstring(string token);
        Task<ApplicationUser> GetUserfromRefreshToken(string token);
        void RemoveAllRefreshTokensForUser(ICollection<RefreshToken> refreshTokens);
    }
}
