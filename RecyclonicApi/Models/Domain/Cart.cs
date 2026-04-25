namespace RecyclonicApi.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFees { get; set; }

        public Guid UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
