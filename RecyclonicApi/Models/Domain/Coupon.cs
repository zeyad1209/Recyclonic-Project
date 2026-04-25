using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        [Range(1,99)]
        public decimal DiscountValue { get; set; }
        public bool isdeliveryfees { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0; 
        public bool IsActive { get; set; } = true;
        public ICollection<Order>? orders { get; set; }
    }
}
