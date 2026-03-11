using PieShopApi.Models;

namespace PieShopApi.Services;

public class InMemoryPieRepository : IPieRepository
{
    private readonly List<Pie> _pies = new();
    private int _nextId = 1;

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