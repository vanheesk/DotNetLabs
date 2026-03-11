using System.Runtime.Serialization;

namespace PieShopApi;

public class XmlResult<T>(T value, int statusCode = 200) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/xml";

        var serializer = new DataContractSerializer(typeof(T));
        await using var ms = new MemoryStream();
        serializer.WriteObject(ms, value);
        ms.Position = 0;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}
