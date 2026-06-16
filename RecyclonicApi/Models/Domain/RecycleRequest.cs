using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclonicApi.Models
{
    public class RecycleRequest
    {
        public Guid Id { get; set; }    
        public string Status { get; set; }
        public decimal? OfferedPrice { get; set; }
        public bool? UserResponse { get; set; }
        public bool IsDelivered { get; set; } = false;
        public decimal? AmountPaidToUser { get; set; }
        public decimal? AmountReceivedFromRecycler { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public string Address { get; set; }
        [Required]
        public Guid EwasteItemId { get; set; }
        [ForeignKey("EwasteItemId")]
        public EwasteItem ewasteItem { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser user { get; set; }
        public Guid? EmployeeId { get; set; }
        public ApplicationUser? Employee { get; set; }
        public Delivery? delivery { get; set; }
    }
}
