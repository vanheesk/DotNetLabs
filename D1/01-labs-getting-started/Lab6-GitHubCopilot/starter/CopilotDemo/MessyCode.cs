namespace CopilotDemo;

public record Order(string Id, decimal Amount, string Category, bool IsPriority);

/// <summary>
/// This class contains intentionally messy code.
/// Use GitHub Copilot to refactor it into clean, readable C#.
/// </summary>
public static class MessyCode
{
    public static string ProcessOrders(List<Order> orders)
    {
        // Calculate total, apply discounts, filter by category — all in one ugly method
        decimal t = 0;
        int c = 0;
        decimal d = 0;
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i] != null)
            {
                if (orders[i].Category == "Electronics")
                {
                    if (orders[i].IsPriority == true)
                    {
                        if (orders[i].Amount > 100)
                        {
                            d = orders[i].Amount * 0.1m;
                            t = t + orders[i].Amount - d;
                            c = c + 1;
                        }
                        else
                        {
                            t = t + orders[i].Amount;
                            c = c + 1;
                        }
                    }
                    else
                    {
                        t = t + orders[i].Amount;
                        c = c + 1;
                    }
                }
                else if (orders[i].Category == "Books")
                {
                    if (orders[i].Amount > 20)
                    {
                        d = orders[i].Amount * 0.05m;
                        t = t + orders[i].Amount - d;
                        c = c + 1;
                    }
                    else
                    {
                        t = t + orders[i].Amount;
                        c = c + 1;
                    }
                }
                else
                {
                    t = t + orders[i].Amount;
                    c = c + 1;
                }
            }
        }
        string r = "Processed " + c.ToString() + " orders, total: $" + t.ToString("F2");
        return r;
    }
}
