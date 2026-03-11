using PieShopApi.Models;

namespace PieShopApi.Services;

public interface IPieRepository
{
    IEnumerable<Pie> GetAll();
    Pie? GetById(int id);
    Pie Add(Pie pie);
    Pie? Update(int id, Pie pie);
    bool Delete(int id);
}