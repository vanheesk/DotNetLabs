namespace PieShopApi.Data;

public class CategoryEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PieEntity> Pies { get; set; } = [];
}
