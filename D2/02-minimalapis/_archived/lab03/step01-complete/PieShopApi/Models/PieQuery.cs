namespace PieShopApi.Models;

public record PieQuery(string? Filter, string? OrderBy, int Page = 1, int PageSize = 10);