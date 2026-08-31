using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Application.Abstractions;

public interface IAuditQueryService
{
    Task<AuditQueryResponse> SearchAsync(AuditQueryRequest request, CancellationToken ct = default);
}
