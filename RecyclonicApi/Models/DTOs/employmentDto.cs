using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models.DTOs
{
    public class employmentDto
    {
        [EmailAddress]
        [Required]
        public string Email {  get; set; }
        [Required]
        public string Role { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
    public class InvitationResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Expired { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool? Status { get; set; }
    }

    public class RespondInvitationDto
    {
        public Guid InvitaionId { get; set; }
        public bool Accepted { get; set; }
    }
}
