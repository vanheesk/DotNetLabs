using PieShopApi.Models;

namespace PieShopApi.Services;

public class InMemoryPieRepository : IPieRepository
{
    private readonly List<Pie> _pies =
    [
        new Pie(1, "Apple Pie", "Our famous apple pie made with fresh apples", 12.95m, true, 1),
        new Pie(2, "Blueberry Cheese Cake", "A creamy blueberry delight", 18.95m, false, 2),
        new Pie(3, "Cherry Pie", "A classic cherry pie with flaky crust", 13.95m, false, 1),
        new Pie(4, "Strawberry Pie", "Fresh strawberry pie with whipped cream", 15.95m, true, 1),
        new Pie(5, "Pecan Pie", "Southern-style pecan pie", 16.95m, false, 1),
        new Pie(6, "Pumpkin Pie", "Seasonal pumpkin pie with spices", 14.95m, false, 1),
        new Pie(7, "Chocolate Cream Cake", "Rich chocolate cream cake", 19.95m, true, 2),
        new Pie(8, "Key Lime Pie", "Tangy key lime pie", 15.95m, false, 1),
    ];

    private int _nextId = 9;

    public IEnumerable<Pie> GetAll() => _pies;
    public Pie? GetById(int id) => _pies.FirstOrDefault(p => p.PieId == id);

    public Pie Add(Pie pie)
    {
        pie = pie with { PieId = _nextId++ };
        _pies.Add(pie);
        return pie;
    }

    public Pie? Update(int id, Pie pie)
    {
        var index = _pies.FindIndex(p => p.PieId == id);
        if (index == -1) return null;
        _pies[index] = pie with { PieId = id };
        return _pies[index];
    }

    public bool Delete(int id)
    {
        var index = _pies.FindIndex(p => p.PieId == id);
        if (index == -1) return false;
        _pies.RemoveAt(index);
        return true;
    }
}
