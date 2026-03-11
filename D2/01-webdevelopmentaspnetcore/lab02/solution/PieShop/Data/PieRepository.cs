using Microsoft.EntityFrameworkCore;
using PieShop.Models;

namespace PieShop.Data;

public class PieRepository : IPieRepository
{
    private readonly PieShopDbContext _context;

    public PieRepository(PieShopDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Pie> AllPies =>
        _context.Pies.Include(p => p.Category).ToList();

    public IEnumerable<Pie> PiesOfTheWeek =>
        _context.Pies.Include(p => p.Category)
            .Where(p => p.IsPieOfTheWeek).ToList();

    public Pie? GetPieById(int pieId) =>
        _context.Pies.Include(p => p.Category)
            .FirstOrDefault(p => p.PieId == pieId);
}
