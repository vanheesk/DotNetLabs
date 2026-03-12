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

    [HttpGet]
    public async Task<ActionResult<string>> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start slow request");

        await Task.Delay(10_000, cancellationToken);

        _logger.LogInformation("End slow request");
        return "slow";
    }
}
