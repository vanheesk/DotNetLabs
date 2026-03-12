using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace PieShopApi.Controllers;

[Route("[controller]")]
[ApiController]
public class DateTimeController : ControllerBase
{
    private readonly IOutputCacheStore _cacheStore;

    public DateTimeController(IOutputCacheStore cacheStore)
    {
        _cacheStore = cacheStore;
    }

    [HttpGet]
    [Route("fromoutputcache")]
    [OutputCache]
    public async Task<IActionResult> GetOutputCache()
    {
        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    [HttpGet]
    [Route("fromoutputcacherevalidation")]
    [OutputCache]
    public async Task<IActionResult> GetOutputCacheRevalidation()
    {
        var etag = $"\"{Guid.NewGuid():n}\"";
        Response.Headers.ETag = etag;

        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    [HttpGet]
    [Route("fromoutputcacheeviction")]
    [OutputCache(Tags = new string[] { "tag-datetime" })]
    public async Task<IActionResult> GetOutputCacheEviction()
    {
        await Task.Delay(5000);

        return Ok(new { DateTime = System.DateTime.Now });
    }

    [HttpPost]
    [Route("purge/{tag}")]
    public async Task<IActionResult> Purge(string tag)
    {
        await _cacheStore.EvictByTagAsync(tag, default);

        return NoContent();
    }
}
