namespace RecyclonicApi.Models.DTOs
{
    public class UpdateDeliveryStatusDto
    {
        public bool IsDelivered { get; set; }
        public decimal? AmountPaidToUser { get; set; }
        public decimal? AmountReceivedFromRecycler { get; set; }
    }
}
