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
    public async Task<ActionResult<IEnumerable<Pie>>> GetAll()
    {
        var pies = await _pieRepository.GetAllPiesAsync();
        return Ok(pies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pie>> GetById(int id)
    {
        var pie = await _pieRepository.GetPieByIdAsync(id);
        if (pie is null)
            return NotFound();
        return Ok(pie);
    }
}
