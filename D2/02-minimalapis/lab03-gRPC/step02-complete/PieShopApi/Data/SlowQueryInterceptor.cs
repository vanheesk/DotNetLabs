using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PieShopApi.Data;

public class SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger, TimeSpan threshold) : DbCommandInterceptor
{
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Duration > threshold)
        {
            logger.LogWarning(
                "Slow query detected ({Duration}ms): {CommandText}",
                eventData.Duration.TotalMilliseconds,
                command.CommandText);
        }
        return ValueTask.FromResult(result);
    }
}
