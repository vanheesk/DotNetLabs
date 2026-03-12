using Microsoft.AspNetCore.Mvc;
using PieShopApi.Models;

namespace PieShopApi.Controllers;

[ApiController]
[Route("pies")]
public class PiesController : ControllerBase
{
    private readonly IPieRepository _pieRepository;

    public PiesController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PieDto>>> GetAll()
    {
        var pies = await _pieRepository.GetAllPiesAsync();
        var dtos = pies.Select(p => new PieDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price
        });
        return Ok(dtos);
    }

    [HttpGet]
    // TODO: Add the [EnableRateLimiting("myWindowLimiter")] attribute here
    // Hint: You need using Microsoft.AspNetCore.RateLimiting;
    [Route("{id:int}", Name = "GetPie")]
    public async Task<ActionResult<PieDto>> GetPie(int id)
    {
        var pie = await _pieRepository.GetPieByIdAsync(id);
        if (pie is null)
            return NotFound();

        return Ok(new PieDto
        {
            Id = pie.Id,
            Name = pie.Name,
            Description = pie.Description,
            Price = pie.Price
        });
    }
}
