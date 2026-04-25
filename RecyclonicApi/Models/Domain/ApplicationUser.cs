using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RecyclonicApi.Models
{

    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Address { get; set; }
        public bool IsfromRecyclonic { get; set; }
        public string role = "User";
        public Cart? cart { get; set; }
        public ICollection<MarketplaceItem>? FavouriteItems { get; set; }
        public ICollection<MarketplaceItem>? MyItems { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<RecycleRequest>? RecycleRequests { get; set; }
        public ICollection<RefreshToken>? refreshTokens { get; set; }
    }
}
