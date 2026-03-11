using PieShopApi.Models;

namespace PieShopApi.Services;

public class InMemoryPieRepository : IPieRepository
{
    private readonly List<Pie> _pies;
    private int _nextId;

    public InMemoryPieRepository()
    {
        _pies =
        [
            new Pie(1, "Apple Pie", "Classic apple pie with cinnamon", 12.95m, true, 1),
            new Pie(2, "Blueberry Cheesecake", "Creamy cheesecake with blueberry topping", 14.50m, false, 2),
            new Pie(3, "Cherry Pie", "Traditional cherry pie with lattice crust", 11.75m, false, 1),
            new Pie(4, "Strawberry Pie", "Fresh strawberry pie with whipped cream", 13.25m, true, 1),
            new Pie(5, "Pumpkin Pie", "Seasonal pumpkin pie with nutmeg", 10.95m, false, 3),
            new Pie(6, "Pecan Pie", "Rich pecan pie with caramel", 15.00m, false, 3),
            new Pie(7, "Lemon Meringue", "Tangy lemon filling with meringue top", 12.50m, true, 1),
            new Pie(8, "Chocolate Cream Pie", "Decadent chocolate cream pie", 13.95m, false, 2)
        ];
        _nextId = _pies.Count + 1;
    }

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
