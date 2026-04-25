namespace RecyclonicApi.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceAtAdd { get; set; }
        public Guid CartId { get; set; } 
        public Guid MarketplaceItemId { get; set; } 
        public Cart Cart { get; set; }
        public MarketplaceItem marketplaceItem { get; set; }
    }
}
