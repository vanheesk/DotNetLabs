using Microsoft.AspNetCore.Mvc;

namespace PieShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DateTimeController : ControllerBase
{
    [HttpGet("fromresponsecache")]
    [ResponseCache(CacheProfileName = "Cache2Minutes")]
    public IActionResult GetWithResponseCache()
    {
        return Ok(new { dateTime = DateTime.Now });
    }

    [HttpGet("fromresponsecachebyid")]
    [ResponseCache(CacheProfileName = "Cache2Minutes", VaryByQueryKeys = new[] { "id" })]
    public IActionResult GetWithResponseCacheById(int id, string user)
    {
        return Ok($"id:{id} - user:{user} - {DateTime.Now}");
    }
}
