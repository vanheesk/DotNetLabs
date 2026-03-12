using Microsoft.AspNetCore.Mvc;

namespace PieShopApi.Controllers;

[Route("[controller]")]
[ApiController]
public class DateTimeController : ControllerBase
{
    // TODO: Add [ResponseCache] attribute with Duration and Location (Part 1)
    // TODO: For Part 3, use [ResponseCache(CacheProfileName = "Cache2Minutes")] instead
    [HttpGet]
    [Route("fromresponsecache")]
    public IActionResult Get()
    {
        return Ok(new { DateTime = System.DateTime.Now });
    }

    // TODO: Add [ResponseCache] attribute (Part 2)
    // TODO: Add VaryByQueryKeys = new string[] { "id" } (Part 2)
    [HttpGet]
    [Route("fromresponsecachebyid")]
    public IActionResult Get(int id, string user)
    {
        return Ok($"{user}: response was generated for Id:{id} at {DateTime.Now}");
    }
}
