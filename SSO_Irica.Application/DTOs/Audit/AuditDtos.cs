namespace SSO_Irica.Application.DTOs.Audit;

public sealed record AuditQueryRequest(
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    string? EventName = null,
    string? UserId = null,
    int Page = 1,
    int PageSize = 50);

public sealed record AuditEventResponse(
    DateTimeOffset? Timestamp,
    string? EventName,
    string? UserId,
    string? Action,
    string? Resource,
    int StatusCode,
    string? IpAddress);

public sealed record AuditQueryResponse(long Total, IReadOnlyList<AuditEventResponse> Items);
