using Microsoft.EntityFrameworkCore;

namespace PieShopApi.Data;

public class PieShopDbContext(DbContextOptions<PieShopDbContext> options) : DbContext(options)
{
    public DbSet<PieEntity> Pies => Set<PieEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PieShopDbContext).Assembly);
    }
}
