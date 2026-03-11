namespace PieShopApi.Models;

public record PieDto(int PieId, string Name, string? ShortDescription, decimal Price, bool IsPieOfTheWeek, string CategoryName);
