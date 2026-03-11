using System.ComponentModel.DataAnnotations;

namespace PieShopBlazor.Models;

public class PieFormModel
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = "";

    [StringLength(200)]
    public string? ShortDescription { get; set; }

    [Range(0.01, 1000)]
    public decimal Price { get; set; }

    public bool IsPieOfTheWeek { get; set; }

    public int CategoryId { get; set; } = 1;
}
