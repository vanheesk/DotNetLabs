namespace PieShopApi.Filters;

public class LoggingEndpointFilter(ILogger<LoggingEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        logger.LogInformation("Filter executing for {Method} {Path}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        var result = await next(context);

        logger.LogInformation("Filter completed");
        return result;
    }
}