namespace PieShopApi.Models;

public record Pie(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, int CategoryId);
