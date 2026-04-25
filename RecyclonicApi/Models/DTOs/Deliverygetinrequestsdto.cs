namespace RecyclonicApi.Models.DTOs
{
    public class Deliverygetinrequestsdto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public string PickUpAddress { get; set; }
        public string? PickUpAddressUrl { get; set; }
        public string? DeliveryEmployeeName { get; set; }
        public ICollection<StatusTrakingdto> StatusTrakingdtos { get; set; }

    }

    public class StatusTrakingdto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Dateofstatus { get; set; }
    }
}
