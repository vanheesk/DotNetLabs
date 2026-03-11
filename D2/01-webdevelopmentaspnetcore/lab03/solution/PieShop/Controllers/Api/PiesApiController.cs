using Microsoft.AspNetCore.Mvc;
using PieShop.Models;

namespace PieShop.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PiesApiController : ControllerBase
{
    private readonly IPieRepository _pieRepository;

    public PiesApiController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_pieRepository.AllPies);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var pie = _pieRepository.GetPieById(id);
        if (pie == null)
            return NotFound();
        return Ok(pie);
    }
}
