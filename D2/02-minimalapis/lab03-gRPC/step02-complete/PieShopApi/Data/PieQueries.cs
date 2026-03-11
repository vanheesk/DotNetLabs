using Microsoft.EntityFrameworkCore;
using PieShopApi.Data;

namespace PieShopApi;

public static class PieQueries
{
    public static readonly Func<PieShopDbContext, int, Task<PieEntity?>> GetById =
        EF.CompileAsyncQuery((PieShopDbContext db, int id) =>
            db.Pies.Include(p => p.Category).FirstOrDefault(p => p.PieId == id));
}
