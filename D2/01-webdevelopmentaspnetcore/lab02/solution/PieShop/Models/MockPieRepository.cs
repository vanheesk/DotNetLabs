namespace PieShop.Models;

public class MockCategoryRepository
{
    public static Category FruitPies { get; } = new()
    {
        CategoryId = 1,
        Name = "Fruit pies",
        Description = "All-fruity pies"
    };

    public static Category CheeseCakes { get; } = new()
    {
        CategoryId = 2,
        Name = "Cheese cakes",
        Description = "Cheesy all the way"
    };
}

public class MockPieRepository : IPieRepository
{
    private readonly List<Pie> _pies;

    public MockPieRepository()
    {
        var fruitCategory = MockCategoryRepository.FruitPies;
        var cheeseCategory = MockCategoryRepository.CheeseCakes;

        _pies = new List<Pie>
        {
            new()
            {
                PieId = 1,
                Name = "Apple Pie",
                ShortDescription = "Our famous apple pie",
                LongDescription = "A classic apple pie made with fresh Granny Smith apples and a flaky crust.",
                Price = 12.95m,
                IsPieOfTheWeek = true,
                CategoryId = 1,
                Category = fruitCategory
            },
            new()
            {
                PieId = 2,
                Name = "Blueberry Cheese Cake",
                ShortDescription = "Delicious blueberry cheese cake",
                LongDescription = "Rich cheese cake topped with fresh blueberries and a graham cracker crust.",
                Price = 18.95m,
                IsPieOfTheWeek = false,
                CategoryId = 2,
                Category = cheeseCategory
            },
            new()
            {
                PieId = 3,
                Name = "Strawberry Pie",
                ShortDescription = "Fresh strawberry pie",
                LongDescription = "Sweet pie packed with juicy strawberries and a buttery crust.",
                Price = 15.95m,
                IsPieOfTheWeek = true,
                CategoryId = 1,
                Category = fruitCategory
            }
        };
    }

    public IEnumerable<Pie> AllPies => _pies;
    public IEnumerable<Pie> PiesOfTheWeek => _pies.Where(p => p.IsPieOfTheWeek);
    public Pie? GetPieById(int pieId) => _pies.FirstOrDefault(p => p.PieId == pieId);
}
