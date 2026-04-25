
using RecyclonicApi.Models;

namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IHelperService
    {
        Task<string> GetHighestRole(ApplicationUser user);
        Task changeRole(ApplicationUser user, string Role);
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string foldername);
    }
}
