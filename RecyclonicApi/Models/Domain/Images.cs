using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models
{
    public class Images
    {
        public Guid Id { get; set; }
        [Required]
        public string Url { get; set; }
        public Guid? EwasteitemId { get; set; }
        public EwasteItem? ewasteItem { get; set; }
        public Guid? MarketPlaceItemId { get; set; }
        public MarketplaceItem? marketplaceItem { get; set; }
    }
}
