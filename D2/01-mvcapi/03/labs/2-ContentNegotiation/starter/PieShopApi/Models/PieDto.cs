namespace PieShopApi.Models;

public class PieDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AllergyItems { get; set; } = [];
}
