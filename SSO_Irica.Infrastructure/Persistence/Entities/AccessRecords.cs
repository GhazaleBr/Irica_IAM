namespace SSO_Irica.Infrastructure.Persistence;

public sealed class ApplicationRecord
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class RoleRecord
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UserRoleRecord
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}

public sealed class ModuleRecord
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
}

public sealed class PermissionRecord
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public sealed class RolePermissionRecord
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int ModuleId { get; set; }
    public int PermissionId { get; set; }
    public bool IsActive { get; set; }
}
