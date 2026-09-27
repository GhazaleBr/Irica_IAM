using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSO_Irica.Application.DTOs.Access;
using SSO_Irica.Application.DTOs.Access.Requests;
using SSO_Irica.Application.DTOs.Organization.Requests;

namespace SSO_Irica.Api.Audit;

public sealed class AuditTarget
{
    public string? EntityType { get; init; }
    public long? EntityId { get; set; }
    public string? EntityKey { get; set; }
    public int? ModuleId { get; set; }
    public IReadOnlyList<int>? ModuleIds { get; set; }
    public IReadOnlyList<int>? PermissionIds { get; set; }
    public int? ApplicationId { get; set; }
    public int? PositionId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? OrganizationTypeId { get; set; }
    public int? ParentOrganizationUnitId { get; set; }
    public string? SubjectUserId { get; set; }

    public static AuditTarget FromRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries)
            ?? Array.Empty<string>();
        var entityType = path.FirstOrDefault(segment => EntityTypes.ContainsKey(segment));
        var target = new AuditTarget { EntityType = entityType is null ? null : EntityTypes[entityType] };
        if (context.Request.RouteValues.TryGetValue("id", out var id) ||
            context.Request.RouteValues.TryGetValue("roleId", out id))
        {
            if (long.TryParse(id?.ToString(), out var numericId)) target.EntityId = numericId;
        }
        return target;
    }

    private static readonly IReadOnlyDictionary<string, string> EntityTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["auth"] = "User",
        ["applications"] = "Application",
        ["modules"] = "Module",
        ["permissions"] = "Permission",
        ["roles"] = "Role",
        ["organization-types"] = "OrganizationType",
        ["organization-units"] = "OrganizationUnit",
        ["positions"] = "Position",
        ["genders"] = "Gender",
        ["user-positions"] = "UserPosition"
    };
}

public sealed class AuditTargetFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var target = AuditTarget.FromRequest(context.HttpContext);
        context.HttpContext.Items[nameof(AuditTarget)] = target;

        foreach (var argument in context.ActionArguments.Values)
        {
            switch (argument)
            {
                case AssignRoleRequest assignment:
                    target.SubjectUserId = assignment.UserId.ToString();
                    break;
                case CreateUserPositionRequest userPosition:
                    target.SubjectUserId = userPosition.UserId.ToString();
                    target.PositionId = userPosition.PositionId;
                    break;
                case UpdateUserPositionRequest userPosition:
                    target.PositionId = userPosition.PositionId;
                    break;
                case CreatePositionRequest position:
                    target.OrganizationUnitId = position.OrganizationUnitId;
                    break;
                case UpdatePositionRequest position:
                    target.OrganizationUnitId = position.OrganizationUnitId;
                    break;
                case CreateOrganizationUnitRequest unit:
                    target.OrganizationTypeId = unit.TypeId;
                    target.ParentOrganizationUnitId = unit.ParentId;
                    break;
                case UpdateOrganizationUnitRequest unit:
                    target.OrganizationTypeId = unit.TypeId;
                    target.ParentOrganizationUnitId = unit.ParentId;
                    break;
                case CreateModuleRequest module:
                    target.ApplicationId = module.ApplicationId;
                    break;
                case UpdateModuleRequest module:
                    target.ApplicationId = module.ApplicationId;
                    break;
                case ReplaceRoleAccessRequest roleAccess:
                    target.ModuleIds = roleAccess.Items.Select(item => item.ModuleId).Distinct().ToArray();
                    target.PermissionIds = roleAccess.Items.Select(item => item.PermissionId).Distinct().ToArray();
                    break;
            }
        }

        if (target.EntityType == "Module" &&
            context.ActionArguments.TryGetValue("applicationId", out var applicationFilter) &&
            applicationFilter is int filteredApplicationId)
            target.ApplicationId = filteredApplicationId;
        if (target.EntityType == "UserPosition" &&
            context.ActionArguments.TryGetValue("userId", out var userFilter) &&
            userFilter is Guid userId)
            target.SubjectUserId = userId.ToString();

        var executed = await next();
        if (executed.Exception is not null || executed.Result is not ObjectResult { Value: { } value }) return;

        var responseId = ReadProperty(value, "Id") ?? ReadProperty(value, $"{target.EntityType}Id");
        if (responseId is int numericResponseId) target.EntityId = numericResponseId;
        else if (responseId is long longResponseId) target.EntityId = longResponseId;
        else if (responseId is Guid guidResponseId) target.EntityKey = guidResponseId.ToString();

        if (ReadProperty(value, "ApplicationId") is int applicationId) target.ApplicationId = applicationId;
        if (ReadProperty(value, "PositionId") is int positionId) target.PositionId = positionId;
        if (ReadProperty(value, "OrganizationUnitId") is int unitId) target.OrganizationUnitId = unitId;
        if (ReadProperty(value, "TypeId") is int typeId && target.EntityType == "OrganizationUnit")
            target.OrganizationTypeId = typeId;
        if (target.EntityType == "Module" && target.EntityId is > 0 and <= int.MaxValue)
            target.ModuleId = (int)target.EntityId.Value;
    }

    private static object? ReadProperty(object value, string propertyName) =>
        value.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            ?.GetValue(value);

}
