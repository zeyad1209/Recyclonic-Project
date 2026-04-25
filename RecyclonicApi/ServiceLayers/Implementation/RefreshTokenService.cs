using RecyclonicApi.Models;
using RecyclonicApi.Repository.Interfaces;
using RecyclonicApi.ServiceLayers.Interfaces;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepo _repo;

        public RefreshTokenService(IRefreshTokenRepo repo)
        {
            _repo = repo;
        }

        public async Task<bool> Add_RefreshTokenNow(string refreshToken, Guid userId)
        {
            var isValid = await _repo.ValidGeneratetoken(refreshToken);
            if (!isValid)
                return false;

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = userId,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(15),
                IsRevoked = false
            };

            await _repo.CreateAsync(newRefreshToken);
            await _repo.Save();

            return true;
        }
    }
}
