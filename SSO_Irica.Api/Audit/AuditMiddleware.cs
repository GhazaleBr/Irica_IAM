using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Api.Audit;

public sealed class AuditMiddleware(RequestDelegate next, IOptions<AuditOptions> options)
{
    public async Task InvokeAsync(HttpContext context, IAuditLogger audit)
    {
        var failed = false;
        try
        {
            await next(context);
        }
        catch
        {
            failed = true;
            throw;
        }
        finally
        {
            if (!context.Request.Path.StartsWithSegments("/swagger"))
            {
                var endpoint = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
                var eventName = context.Request.Path.StartsWithSegments("/api/auth/login") ? "login"
                    : context.Request.Path.StartsWithSegments("/api/auth/verify-two-factor")
                        ? context.Response.StatusCode < 400 && !failed ? "two_factor_verified" : "two_factor_failed"
                    : context.Request.Path.StartsWithSegments("/api/auth/logout") ? "logout"
                    : context.Request.Path.StartsWithSegments("/api/auth/refresh") ? "token_refresh"
                    : endpoint is null ? "http_request" : $"{endpoint.ControllerName}.{endpoint.ActionName}";
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? context.Items["AuditUserId"] as string;
                var target = context.Items[nameof(AuditTarget)] as AuditTarget ?? AuditTarget.FromRequest(context);
                var clientSystem = context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirstValue("client_id") ?? context.User.FindFirstValue("azp")
                    : null;
                await audit.WriteAsync(new AuditEvent(eventName, userId,
                    $"{context.Request.Method} {context.Request.Path}", context.Request.Path,
                    failed && context.Response.StatusCode < 400 ? 500 : context.Response.StatusCode,
                    context.Connection.RemoteIpAddress?.ToString())
                {
                    ServiceName = options.Value.ServiceName,
                    ActorType = userId is null ? "anonymous" : "user",
                    ActorSystem = clientSystem ?? options.Value.ServiceName,
                    EntityType = target?.EntityType,
                    EntityId = target?.EntityId,
                    EntityKey = target?.EntityKey ?? (target?.EntityType == "User" ? userId : null),
                    ModuleId = target?.ModuleId ?? (target?.EntityType == "Module" && target.EntityId is > 0 and <= int.MaxValue
                        ? (int)target.EntityId.Value : options.Value.ModuleId > 0 ? options.Value.ModuleId : null),
                    ModuleIds = target?.ModuleIds,
                    PermissionIds = target?.PermissionIds,
                    ApplicationId = target?.ApplicationId,
                    PositionId = target?.PositionId,
                    OrganizationUnitId = target?.OrganizationUnitId,
                    OrganizationTypeId = target?.OrganizationTypeId,
                    ParentOrganizationUnitId = target?.ParentOrganizationUnitId,
                    SubjectUserId = target?.SubjectUserId,
                    TraceId = context.TraceIdentifier
                }, CancellationToken.None);
            }
        }
    }
}
