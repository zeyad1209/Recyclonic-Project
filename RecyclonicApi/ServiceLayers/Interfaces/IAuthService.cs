using RecyclonicApi.HelperServices;
using RecyclonicApi.Models.DTOs;

namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult> Register(RegisterDto dtouser);
        Task<ServiceResult> VerifyUser(VerifyUserDto dto);
        Task<ServiceResult> Login(LoginDto dto);
        Task<ServiceResult> Refresh(string oldrefreshtoken);
        Task<ServiceResult> Log_out(string oldRefreshToken);
        Task<ServiceResult> Log_out_All(string UserId);
        Task<ServiceResult> Change_Password(Change_Password_Dto dto, string userId);
        Task<ServiceResult> Forget_Password(string email);
        Task<ServiceResult> Reset_Password(Reset_Password_Dto dto);
        Task<ServiceResult> Editorcompletedata(string UserId, editorcompleteprofiledata dto);
        Task<ServiceResult> Getmyprofile(string UserId);
    }
}
