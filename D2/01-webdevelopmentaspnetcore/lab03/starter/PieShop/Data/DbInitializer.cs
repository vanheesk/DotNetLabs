using PieShop.Models;

namespace PieShop.Data;

public static class DbInitializer
{
    public static void Seed(PieShopDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Pies.Any())
            return; // Already seeded

        var fruitCategory = new Category
        {
            Name = "Fruit pies",
            Description = "All-fruity pies"
        };

        var cheeseCategory = new Category
        {
            Name = "Cheese cakes",
            Description = "Cheesy all the way"
        };

        context.Categories.AddRange(fruitCategory, cheeseCategory);

        context.Pies.AddRange(
            new Pie
            {
                Name = "Apple Pie",
                ShortDescription = "Our famous apple pie",
                LongDescription = "A classic apple pie made with fresh Granny Smith apples and a flaky crust.",
                Price = 12.95m,
                IsPieOfTheWeek = true,
                Category = fruitCategory
            },
            new Pie
            {
                Name = "Blueberry Cheese Cake",
                ShortDescription = "Delicious blueberry cheese cake",
                LongDescription = "Rich cheese cake topped with fresh blueberries and a graham cracker crust.",
                Price = 18.95m,
                IsPieOfTheWeek = false,
                Category = cheeseCategory
            },
            new Pie
            {
                Name = "Strawberry Pie",
                ShortDescription = "Fresh strawberry pie",
                LongDescription = "Sweet pie packed with juicy strawberries and a buttery crust.",
                Price = 15.95m,
                IsPieOfTheWeek = true,
                Category = fruitCategory
            },
            new Pie
            {
                Name = "Cherry Pie",
                ShortDescription = "Classic cherry pie",
                LongDescription = "Traditional cherry pie with a flaky crust and tart cherry filling.",
                Price = 13.95m,
                IsPieOfTheWeek = false,
                Category = fruitCategory
            },
            new Pie
            {
                Name = "Pumpkin Cheese Cake",
                ShortDescription = "Seasonal pumpkin cheese cake",
                LongDescription = "Creamy pumpkin cheese cake with a graham cracker crust and warm spices.",
                Price = 16.95m,
                IsPieOfTheWeek = true,
                Category = cheeseCategory
            }
        );

        context.SaveChanges();
    }
}
