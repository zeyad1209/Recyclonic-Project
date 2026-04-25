using RecyclonicApi.Models.Domain;

namespace RecyclonicApi.Repository.Interfaces
{
    public interface IInvitationRepo : IGenericRepo<Invitation>
    {
        Task<IEnumerable<Invitation>> GetmyInvitationsAsync(string Email);
        Task<Invitation> GetInvitationbyIdAsync(Guid Id);
    }
}
