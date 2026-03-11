namespace PieShop.Services;

public class InviteLinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InviteLinkService(LinkGenerator linkGenerator,
                              IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GeneratePieLink(int pieId)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        return _linkGenerator.GetUriByAction(
            httpContext,
            action: "Details",
            controller: "Pie",
            values: new { id = pieId });
    }
}
