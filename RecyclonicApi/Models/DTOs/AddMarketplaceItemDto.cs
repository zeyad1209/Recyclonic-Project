using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models.DTOs
{
    public class AddMarketplaceItemDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; } = 1;

        [Required]
        public List<IFormFile> Images { get; set; }


    }
    public class MarketplaceItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int Stock { get; set; }

        public bool IsAvailable { get; set; }

        public Guid SellerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> ImagesUrls { get; set; }
        public bool isfavourite { get; set; } = false;
    }
    public class UpdateMarketplaceItemDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public List<IFormFile>? Images { get; set; }
    }

}