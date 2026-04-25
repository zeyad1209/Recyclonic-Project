using RecyclonicApi.HelperServices;
using RecyclonicApi.Models.DTOs;

namespace RecyclonicApi.ServiceLayers.Interfaces
{
    public interface IEwasteService
    {
        Task<ServiceResult> RecycleEwaste(string UserId, RecycleEwastePostDto dto);
        Task<ServiceResult> GetRecycleWastestorespondit();
        Task<ServiceResult> RespondRecycleRequest(string EmployeeId, string status, Guid RequestId, decimal? price);
        Task<ServiceResult> Userresponsetherequest(string UserId, Guid RequestId, bool accept);
        Task<ServiceResult> Getallrequestforuser(string UserId);
        Task<ServiceResult> Cancel_Request(string UserId, Guid RequestId);
        Task<ServiceResult> Getallrequestforadmin(string adminId);
    }
}
