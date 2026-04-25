namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<bool> Add_RefreshTokenNow(string refreshToken, Guid userId);
    }
}
