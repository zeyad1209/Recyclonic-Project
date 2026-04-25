using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclonicApi.Models.Domain
{
    public class Invitation
    {
        public Guid Id { get; set; }
        [Required]
        public string Role {  get; set; }
        public bool Expired {  get; set; } = false;
        public DateTime CreateddDate { get; set; } = DateTime.Now;
        public bool? status { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public Guid AdminId { get; set; }
        [ForeignKey("AdminId")]
        public ApplicationUser Admin { get; set; }

    }
}
