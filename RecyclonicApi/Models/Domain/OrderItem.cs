namespace RecyclonicApi.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceAtPurchase { get; set; } 

        public Guid OrderId { get; set; }
        public Guid MarketplaceItemId { get; set; }

        public Order Order { get; set; }
        public MarketplaceItem marketplaceItem { get; set; }
    }
}
