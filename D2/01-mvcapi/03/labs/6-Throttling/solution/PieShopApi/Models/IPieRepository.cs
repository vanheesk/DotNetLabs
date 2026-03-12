namespace PieShopApi.Models;

public interface IPieRepository
{
    Task<IEnumerable<Pie>> GetAllPiesAsync();
    Task<Pie?> GetPieByIdAsync(int id);
}
