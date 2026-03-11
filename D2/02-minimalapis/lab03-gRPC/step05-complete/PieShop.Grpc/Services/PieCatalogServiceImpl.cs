using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PieShopApi.Data;

namespace PieShop.Grpc.Services;

public class PieCatalogServiceImpl(PieShopDbContext db, ILogger<PieCatalogServiceImpl> logger)
    : PieCatalogService.PieCatalogServiceBase
{
    public override async Task<PieReply> GetPie(GetPieRequest request, ServerCallContext context)
    {
        logger.LogInformation("gRPC GetPie called for ID {PieId}", request.PieId);

        var pie = await db.Pies.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.PieId == request.PieId, context.CancellationToken);

        if (pie is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Pie {request.PieId} not found"));

        return MapToReply(pie);
    }

    public override async Task<ListPiesReply> ListPies(ListPiesRequest request, ServerCallContext context)
    {
        logger.LogInformation("gRPC ListPies called");

        var pies = await BuildQuery(request).ToListAsync(context.CancellationToken);

        var reply = new ListPiesReply();
        reply.Pies.AddRange(pies.Select(MapToReply));
        return reply;
    }

    public override async Task GetPieStream(
        ListPiesRequest request,
        IServerStreamWriter<PieReply> responseStream,
        ServerCallContext context)
    {
        logger.LogInformation("gRPC GetPieStream started");

        var pies = BuildQuery(request);

        await foreach (var pie in pies.AsAsyncEnumerable().WithCancellation(context.CancellationToken))
        {
            await responseStream.WriteAsync(MapToReply(pie), context.CancellationToken);
            logger.LogInformation("Streamed pie {PieId}: {Name}", pie.PieId, pie.Name);
        }

        logger.LogInformation("gRPC GetPieStream completed");
    }

    private IQueryable<PieEntity> BuildQuery(ListPiesRequest request)
    {
        IQueryable<PieEntity> query = db.Pies.Include(p => p.Category).OrderBy(p => p.PieId);

        if (!string.IsNullOrEmpty(request.Filter))
            query = query.Where(p => p.Name.Contains(request.Filter));

        if (request.AfterId > 0)
            query = query.Where(p => p.PieId > request.AfterId);

        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        return query.Take(pageSize);
    }

    private static PieReply MapToReply(PieEntity pie) => new()
    {
        PieId = pie.PieId,
        Name = pie.Name,
        ShortDescription = pie.ShortDescription ?? "",
        Price = (double)pie.Price,
        IsPieOfTheWeek = pie.IsPieOfTheWeek,
        CategoryName = pie.Category.Name
    };
}
