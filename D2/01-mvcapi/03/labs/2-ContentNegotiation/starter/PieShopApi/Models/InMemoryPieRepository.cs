namespace PieShopApi.Models;

public class InMemoryPieRepository : IPieRepository
{
    private static readonly List<Pie> _pies =
    [
        new Pie { Id = 1, Name = "Apple Pie", Description = "A classic apple pie", Price = 12.95m, AllergyItems = ["Gluten", "Dairy"] },
        new Pie { Id = 2, Name = "Blueberry Cheese Cake", Description = "A creamy blueberry cheese cake", Price = 15.95m, AllergyItems = ["Gluten", "Dairy", "Eggs"] },
        new Pie { Id = 3, Name = "Cherry Pie", Description = "A sweet cherry pie", Price = 13.95m, AllergyItems = ["Gluten"] },
        new Pie { Id = 4, Name = "Pumpkin Pie", Description = "A seasonal pumpkin pie", Price = 11.95m, AllergyItems = ["Gluten", "Dairy", "Eggs"] },
    ];

    public IEnumerable<PieDto> GetAll()
    {
        return _pies.Select(p => new PieDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            AllergyItems = p.AllergyItems
        });
    }

    public PieDto? GetById(int id)
    {
        var pie = _pies.FirstOrDefault(p => p.Id == id);
        if (pie is null) return null;

        return new PieDto
        {
            Id = pie.Id,
            Name = pie.Name,
            Description = pie.Description,
            AllergyItems = pie.AllergyItems
        };
    }
}
