using Microsoft.AspNetCore.Mvc;

namespace AsyncApi.Controllers;

[ApiController]
[Route("slow")]
public class SlowController : ControllerBase
{
    private readonly ILogger<SlowController> _logger;

    public SlowController(ILogger<SlowController> logger)
    {
        _logger = logger;
    }

    // TODO: Add a CancellationToken parameter and pass it to Task.Delay()
    // This allows the server to stop processing when the client disconnects.
    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        _logger.LogInformation("Start slow request");

        await Task.Delay(10_000);

        _logger.LogInformation("End slow request");
        return "slow";
    }
}
