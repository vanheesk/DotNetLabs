using Microsoft.AspNetCore.Mvc;

namespace PieShopApi.Controllers;

[Route("[controller]")]
[ApiController]
public class DateTimeController : ControllerBase
{
    // TODO: Inject IOutputCacheStore for cache eviction (Part 4)

    // TODO: Add [OutputCache] attribute (Part 1)
    // TODO: Add VaryByHeaderNames for header variation (Part 2)
    [HttpGet]
    [Route("fromoutputcache")]
    public async Task<IActionResult> GetOutputCache()
    {
        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    // TODO: Add [OutputCache] attribute for revalidation (Part 3)
    [HttpGet]
    [Route("fromoutputcacherevalidation")]
    public async Task<IActionResult> GetOutputCacheRevalidation()
    {
        var etag = $"\"{Guid.NewGuid():n}\"";
        Response.Headers.ETag = etag;

        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    // TODO: Add [OutputCache] with Tags for eviction (Part 4)
    [HttpGet]
    [Route("fromoutputcacheeviction")]
    public async Task<IActionResult> GetOutputCacheEviction()
    {
        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    // TODO: Implement cache eviction using IOutputCacheStore (Part 4)
    [HttpPost]
    [Route("purge/{tag}")]
    public IActionResult Purge(string tag)
    {
        // TODO: Use _cacheStore.EvictByTagAsync(tag, default) to purge the cache
        return NoContent();
    }
}
