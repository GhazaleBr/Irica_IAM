using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Application.Abstractions;

public interface IAuditLogger
{
    Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
