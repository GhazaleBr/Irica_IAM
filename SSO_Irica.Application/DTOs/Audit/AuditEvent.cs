namespace SSO_Irica.Application.DTOs.Audit;

public sealed record AuditEvent(
    string EventName,
    string? UserId,
    string? Action,
    string? Resource,
    int StatusCode,
    string? IpAddress)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public string ServiceName { get; init; } = "IAM";
    public string ActorType { get; init; } = "anonymous";
    public string? ActorSystem { get; init; }
    public string? EntityType { get; init; }
    public long? EntityId { get; init; }
    public string? EntityKey { get; init; }
    public int? ModuleId { get; init; }
    public IReadOnlyList<int>? ModuleIds { get; init; }
    public IReadOnlyList<int>? PermissionIds { get; init; }
    public int? ApplicationId { get; init; }
    public int? PositionId { get; init; }
    public int? OrganizationUnitId { get; init; }
    public int? OrganizationTypeId { get; init; }
    public int? ParentOrganizationUnitId { get; init; }
    public string? SubjectUserId { get; init; }
    public string? TraceId { get; init; }
}
