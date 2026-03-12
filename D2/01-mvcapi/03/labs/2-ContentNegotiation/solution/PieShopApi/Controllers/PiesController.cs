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
    public ActionResult<IEnumerable<PieDto>> GetAll()
    {
        return Ok(_pieRepository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<PieDto> GetById(int id)
    {
        var pie = _pieRepository.GetById(id);
        if (pie is null)
            return NotFound();

        return Ok(pie);
    }
}
