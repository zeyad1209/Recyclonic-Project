namespace RecyclonicApi.Models
{
    public class Delivery
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; } 
        public string PickUpAddress { get; set; }
        public string? DeliveredAddress { get; set; }
        public string? PickUpAddressUrl { get; set; }
        public string? DeliveredAddressUrl { get; set; }

        public Guid? RecycleRequestId { get; set; } 
        public Guid? DeliveryUserId { get; set; } 
        public Guid? TransactionId { get; set; }
        public RecycleRequest? RecycleRequest { get; set; }
        public Transaction? transaction { get; set; }
        public ApplicationUser? DeliveryUser { get; set; }
        public ICollection<StatusTraking>? statusTraking { get; set; }

    }
}
