using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Access;

namespace SSO_Irica.Api.Controllers;

[ApiController]
[Route("api/admin/access")]
[Authorize(Roles = "Admin")]
public sealed class RoleAccessController(IRoleAccessService access) : ControllerBase
{
    [HttpGet("catalog/modules")]
    public async Task<IActionResult> Modules(CancellationToken cancellationToken) =>
        Ok(await access.GetModulesAsync(cancellationToken));

    [HttpGet("catalog/permissions")]
    public async Task<IActionResult> Permissions(CancellationToken cancellationToken) =>
        Ok(await access.GetPermissionsAsync(cancellationToken));

    [HttpPost("catalog/modules")]
    public async Task<IActionResult> CreateModule(CreateAccessCatalogRequest request, CancellationToken cancellationToken) =>
        Ok(new { moduleId = await access.CreateModuleAsync(request.Code, request.Title, cancellationToken) });

    [HttpPost("catalog/permissions")]
    public async Task<IActionResult> CreatePermission(CreateAccessCatalogRequest request, CancellationToken cancellationToken) =>
        Ok(new { permissionId = await access.CreatePermissionAsync(request.Code, request.Title, cancellationToken) });

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole(CreateRoleRequest request, CancellationToken cancellationToken) =>
        Ok(new { roleId = await access.CreateRoleAsync(request.Code, request.Title, request.Description, cancellationToken) });

    [HttpPost("roles/{roleId:int}/users")]
    public async Task<IActionResult> AssignUser(int roleId, AssignRoleRequest request, CancellationToken cancellationToken)
    {
        await access.AssignRoleAsync(roleId, request.UserId, cancellationToken);
        return NoContent();
    }

    [HttpPut("roles/{roleId:int}/access")]
    public async Task<IActionResult> ReplaceAccess(int roleId, ReplaceRoleAccessRequest request, CancellationToken cancellationToken)
    {
        await access.ReplaceAccessAsync(roleId, request.Items, cancellationToken);
        return NoContent();
    }
}
