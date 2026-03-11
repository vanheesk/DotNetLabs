using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PieShopApi.Data;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

        builder.HasData(
            new CategoryEntity { CategoryId = 1, Name = "Fruit Pies", Description = "Pies made with fresh fruit" },
            new CategoryEntity { CategoryId = 2, Name = "Cheese Cakes", Description = "Creamy cheesecakes" },
            new CategoryEntity { CategoryId = 3, Name = "Seasonal Pies", Description = "Special seasonal favorites" }
        );
    }
}
