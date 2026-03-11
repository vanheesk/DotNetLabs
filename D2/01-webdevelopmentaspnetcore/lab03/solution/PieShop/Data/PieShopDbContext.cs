using Microsoft.EntityFrameworkCore;
using PieShop.Models;

namespace PieShop.Data;

public class PieShopDbContext : DbContext
{
    public PieShopDbContext(DbContextOptions<PieShopDbContext> options)
        : base(options) { }

    public DbSet<Pie> Pies => Set<Pie>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pie>()
            .HasIndex(p => p.Name);

        modelBuilder.Entity<Pie>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Pies)
            .HasForeignKey(p => p.CategoryId);
    }
}
