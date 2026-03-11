using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PieShop.Models;

public class Pie
{
    public int PieId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ImageThumbnailUrl { get; set; }

    public bool IsPieOfTheWeek { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
