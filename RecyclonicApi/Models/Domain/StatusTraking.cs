namespace RecyclonicApi.Models
{
    public class StatusTraking
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Dateofstatus { get; set; }
        public Guid DeliveryId { get; set; }
        public Delivery delivery { get; set; }

    }
}
