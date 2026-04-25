namespace RecyclonicApi.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmountBeforeCopoun { get; set; }
        public decimal TotalAmountAfterCopoun { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Status { get; set; } = "Pending";
        public decimal deliveryfees { get; set; }
        public Guid? CouponId { get; set; }
        public Coupon? Coupon { get; set; }
        public Guid BuyerId { get; set; }
        public ApplicationUser Buyer { get; set; }
        public Transaction? transaction { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
