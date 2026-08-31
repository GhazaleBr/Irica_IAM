using System.Security.Claims;
using SSO_Irica.Application.Abstractions;

namespace SSO_Irica.Api.Audit;

public sealed class AuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAuditLogger audit)
    {
        await next(context);
        if (context.Request.Path.StartsWithSegments("/swagger")) return;
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var eventName = context.Request.Path.StartsWithSegments("/api/auth/login") ? "login"
            : context.Request.Path.StartsWithSegments("/api/auth/verify-two-factor") ? "two_factor_verified"
            : context.Request.Path.StartsWithSegments("/api/auth/logout") ? "logout"
            : context.Request.Path.StartsWithSegments("/api/auth/refresh") ? "token_refresh"
            : "http_request";
        await audit.WriteAsync(eventName, userId, $"{context.Request.Method} {context.Request.Path}",
            context.Request.Path, context.Response.StatusCode,
            context.Connection.RemoteIpAddress?.ToString(), CancellationToken.None);
    }
}
