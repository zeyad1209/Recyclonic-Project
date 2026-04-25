using RecyclonicApi.HelperServices;
using RecyclonicApi.Models.DTOs;

namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IemploymentService
    {
        Task<ServiceResult> employmentUser(string AdminId, employmentDto dto);
        Task<ServiceResult> GetMyInvitations(string UserId);
        Task<ServiceResult> RespondInvitation(string UserId, RespondInvitationDto dto);
    }
}
