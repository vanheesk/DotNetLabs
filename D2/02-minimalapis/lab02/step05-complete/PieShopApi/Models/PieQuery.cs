namespace PieShopApi.Models;

public record PieQuery(string? Filter, string? OrderBy, int? AfterId, int PageSize = 10);
