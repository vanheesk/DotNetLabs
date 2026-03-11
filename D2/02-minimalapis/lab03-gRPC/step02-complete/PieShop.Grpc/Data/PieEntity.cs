namespace PieShopApi.Data;

public class PieEntity
{
    public int PieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsPieOfTheWeek { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; } = null!;
}
