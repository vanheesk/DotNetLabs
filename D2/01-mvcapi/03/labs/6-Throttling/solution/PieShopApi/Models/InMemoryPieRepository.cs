namespace PieShopApi.Models;

public class InMemoryPieRepository : IPieRepository
{
    private readonly List<Pie> _pies =
    [
        new Pie { Id = 1, Name = "Apple Pie", Description = "A classic apple pie", Price = 12.95m },
        new Pie { Id = 2, Name = "Blueberry Cheese Cake", Description = "A delicious blueberry cheese cake", Price = 18.95m },
        new Pie { Id = 3, Name = "Strawberry Pie", Description = "A summer strawberry pie", Price = 15.95m }
    ];

    public async Task<IEnumerable<Pie>> GetAllPiesAsync()
    {
        await Task.Delay(100);
        return _pies;
    }

    public async Task<Pie?> GetPieByIdAsync(int id)
    {
        await Task.Delay(100);
        return _pies.FirstOrDefault(p => p.Id == id);
    }
}
