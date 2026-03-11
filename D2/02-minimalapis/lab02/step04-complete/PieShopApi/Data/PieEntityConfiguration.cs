using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PieShopApi.Data;

public class PieEntityConfiguration : IEntityTypeConfiguration<PieEntity>
{
    public void Configure(EntityTypeBuilder<PieEntity> builder)
    {
        builder.HasKey(p => p.PieId);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ShortDescription).HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(8,2)");
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Pies)
               .HasForeignKey(p => p.CategoryId);

        builder.HasData(
            new PieEntity { PieId = 1, Name = "Apple Pie", ShortDescription = "Classic apple pie with cinnamon", Price = 12.95m, IsPieOfTheWeek = true, CategoryId = 1 },
            new PieEntity { PieId = 2, Name = "Blueberry Cheesecake", ShortDescription = "Creamy cheesecake with blueberry topping", Price = 14.50m, CategoryId = 2 },
            new PieEntity { PieId = 3, Name = "Cherry Pie", ShortDescription = "Traditional cherry pie with lattice crust", Price = 11.75m, CategoryId = 1 },
            new PieEntity { PieId = 4, Name = "Strawberry Pie", ShortDescription = "Fresh strawberry pie with whipped cream", Price = 13.25m, IsPieOfTheWeek = true, CategoryId = 1 },
            new PieEntity { PieId = 5, Name = "Pumpkin Pie", ShortDescription = "Seasonal pumpkin pie with nutmeg", Price = 10.95m, CategoryId = 3 },
            new PieEntity { PieId = 6, Name = "Pecan Pie", ShortDescription = "Rich pecan pie with caramel", Price = 15.00m, CategoryId = 3 },
            new PieEntity { PieId = 7, Name = "Lemon Meringue", ShortDescription = "Tangy lemon filling with meringue top", Price = 12.50m, IsPieOfTheWeek = true, CategoryId = 1 },
            new PieEntity { PieId = 8, Name = "Chocolate Cream Pie", ShortDescription = "Decadent chocolate cream pie", Price = 13.95m, CategoryId = 2 }
        );
    }
}
