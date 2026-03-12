using AsyncApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsyncApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PiesController : ControllerBase
{
    private readonly IPieRepository _pieRepository;

    public PiesController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    // TODO: Make this method async
    // 1. Add the 'async' keyword
    // 2. Change the return type to Task<ActionResult<IEnumerable<Pie>>>
    // 3. Use 'await' when calling GetAllPiesAsync()
    [HttpGet]
    public ActionResult<IEnumerable<Pie>> GetAll()
    {
        var pies = _pieRepository.GetAllPiesAsync().Result;
        return Ok(pies);
    }

    // TODO: Make this method async
    // 1. Add the 'async' keyword
    // 2. Change the return type to Task<ActionResult<Pie>>
    // 3. Use 'await' when calling GetPieByIdAsync()
    [HttpGet("{id:int}")]
    public ActionResult<Pie> GetById(int id)
    {
        var pie = _pieRepository.GetPieByIdAsync(id).Result;
        if (pie is null)
            return NotFound();
        return Ok(pie);
    }
}
