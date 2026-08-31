namespace SSO_Irica.Infrastructure.Persistence;

public sealed class GenderRecord
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
}

public sealed class PositionRecord
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrganizationUnitId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UserPositionRecord
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int PositionId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public bool IsActive { get; set; }
}

public sealed class OrganizationUnitRecord
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public bool IsActive { get; set; }
}

public sealed class OrganizationTypeRecord
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class ActionLogRecord
{
    public int Id { get; set; }
    public long EntityId { get; set; }
    public int ModulesId { get; set; }
}
