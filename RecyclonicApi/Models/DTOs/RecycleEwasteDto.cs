using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models.DTOs
{
    public class RecycleEwastePostDto
    {
        //[Required]
        //public string Model { get; set; }
        [Required]
        public string Condition { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public List<IFormFile> Images { get; set; }
        //public DateTime SubmissionDate { get; set; } = DateTime.Now;
        [Required]
        public decimal Weight { get; set; }
        //[Required]
        //public Guid TypeId { get; set; }
        //[Required]
        //public Guid BrandId { get; set; }
        public string Address { get; set; }
        //public string ?AddressUrl { get; set; }


    }
    public class RecycleEwasteGetDto
    {
        public Guid Id { get; set; }
        //public string Model { get; set; }
        public string Condition { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal Weight { get; set; }
        //public string Typename { get; set; }
        //public string Brandname { get; set; }
        public string Address { get; set; }

    }
    public class RecycleEwasteGetDtotouser
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Model { get; set; }
        public string Condition { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal Weight { get; set; }
        public string Typename { get; set; }
        public string Brandname { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public decimal? OfferedPrice { get; set; }
        public bool? UserResponse { get; set; }
        public string? EmployeeName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDelivered { get; set; }
        public decimal? AmountPaidToUser { get; set; }
        public decimal? AmountReceivedFromRecycler { get; set; }
        public Deliverygetinrequestsdto? delivery { get; set; } // we will convert it to dto later
    }
}
