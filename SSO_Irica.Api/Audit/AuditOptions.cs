namespace SSO_Irica.Api.Audit;

public sealed class AuditOptions
{
    public const string SectionName = "Audit";
    public string ServiceName { get; init; } = "IAM";
    public int ModuleId { get; init; }
}
