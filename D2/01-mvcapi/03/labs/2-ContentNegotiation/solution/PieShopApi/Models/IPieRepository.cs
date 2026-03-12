namespace PieShopApi.Models;

public interface IPieRepository
{
    IEnumerable<PieDto> GetAll();
    PieDto? GetById(int id);
}
