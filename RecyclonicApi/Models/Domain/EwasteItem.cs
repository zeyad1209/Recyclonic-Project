using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclonicApi.Models;

public class EwasteItem
{
    [Key]
    public Guid Id { get; set; }
    //[StringLength(45)]
    //public string Model { get; set; }
    [StringLength(50)]
    [Required]
    public string Condition { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public List<Images> ImagesUrl { get; set; }
    public DateTime SubmissionDate { get; set; } = DateTime.Now;
    [Required]
    public decimal weight { get; set; }
    //[Required]
    //public Guid TypeId { get; set; }
    //[Required]
    //public Guid BrandId { get; set; }
    //[ForeignKey("TypeId")]
    //public EwasteItemType type { get; set; }
    //[ForeignKey("BrandId")]
    //public EwasteItemBrand brand { get; set; }
    public RecycleRequest? recycleRequest { get; set; }
}
