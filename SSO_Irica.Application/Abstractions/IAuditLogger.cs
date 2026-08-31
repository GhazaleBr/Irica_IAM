namespace SSO_Irica.Application.Abstractions;

public interface IAuditLogger
{
    Task WriteAsync(string eventName, string? userId, string? action, string? resource,
        int statusCode, string? ipAddress, CancellationToken cancellationToken = default);
}
