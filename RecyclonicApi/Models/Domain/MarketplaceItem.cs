using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecyclonicApi.Models;

public class MarketplaceItem
{
    [Key]
    public Guid Id { get; set; }


    [StringLength(255)]
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Price { get; set; }

    public string? Description { get; set; }
    [DefaultValue(1)]
    public int Stock { get; set; }
    [Required]
    public List<Images> ImagesUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required]
    public bool isfromrecyclonic { get; set; }
    [Required]
    public bool isaviable { get; set; }
    [Required]
    public Guid SellerId { get; set; }
    [ForeignKey("SellerId")]
    public ApplicationUser seller {  get; set; }
    public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    public ICollection<ApplicationUser>? FavoritedBy { get; set; }

}
