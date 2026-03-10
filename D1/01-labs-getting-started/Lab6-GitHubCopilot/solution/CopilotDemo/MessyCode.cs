namespace CopilotDemo;

public record Order(string Id, decimal Amount, string Category, bool IsPriority);

/// <summary>
/// Refactored version of the messy order processing code (SOLUTION).
/// </summary>
public static class MessyCode
{
    private const decimal ElectronicsDiscountRate = 0.10m;
    private const decimal ElectronicsDiscountThreshold = 100m;
    private const decimal BooksDiscountRate = 0.05m;
    private const decimal BooksDiscountThreshold = 20m;

    public static string ProcessOrders(List<Order> orders)
    {
        var validOrders = orders.Where(o => o is not null);

        decimal total = validOrders.Sum(order => order.Amount - CalculateDiscount(order));
        int count = validOrders.Count();

        return $"Processed {count} orders, total: ${total:F2}";
    }

    private static decimal CalculateDiscount(Order order) => order.Category switch
    {
        "Electronics" when order.IsPriority && order.Amount > ElectronicsDiscountThreshold
            => order.Amount * ElectronicsDiscountRate,
        "Books" when order.Amount > BooksDiscountThreshold
            => order.Amount * BooksDiscountRate,
        _ => 0m
    };
}
