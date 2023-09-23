using AdBlock.Classes;
using Microsoft.AspNetCore.Builder;

namespace AdBlock.Extensions;

public static class ProxyMiddlewareExtension
{
    public static IApplicationBuilder UseProxy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ProxyMiddleware>();
    }
}