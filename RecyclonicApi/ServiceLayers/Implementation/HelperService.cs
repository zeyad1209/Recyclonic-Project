using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Models;
using RecyclonicApi.ServiceLayers.Interfaces;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class HelperService : IHelperService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly IWebHostEnvironment _env;
        public HelperService(
            UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        public async Task<string> GetHighestRole(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var rolepirority = new List<string> { "Admin", "Employee", "Delivery", "User" };
            var highestrole = rolepirority.FirstOrDefault(r => roles.Contains(r));
            return highestrole ?? string.Empty;
        }
        public async Task changeRole(ApplicationUser user, string Role)
        {
            await _userManager.AddToRoleAsync(user, Role);
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, Role);
        }
        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files , string  foldername)
        {
            var urls = new List<string>();
            string folder = Path.Combine(_env.WebRootPath, $"Images/{foldername}");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var img in files)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                urls.Add($"https://recyclonicapp.runasp.net/Images/{foldername}/{fileName}");
            }

            return urls;
        }

    }
}
