using System.ComponentModel.DataAnnotations;

namespace PieShop.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public List<Pie> Pies { get; set; } = new();
}
