using System.ComponentModel.DataAnnotations;

namespace PieShopApi.Models;

public sealed record UpdatePieRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Name,
    [StringLength(200)] string? ShortDescription,
    [Range(0.01, 1000)] decimal Price,
    bool IsPieOfTheWeek = false,
    int CategoryId = 1);
