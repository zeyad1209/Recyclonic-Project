namespace RecyclonicApi.Models.DTOs
{
    public class GetCartdto
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFees { get; set; }  = 0;
        public ICollection<CartItemdto>? Items { get; set; }
    }

    public class CartItemdto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceAtAdd { get; set; }
        public MarketplaceItemDto marketplaceItem { get; set; }

    }
}
