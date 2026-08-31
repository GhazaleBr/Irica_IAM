using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Organization.Requests;

namespace SSO_Irica.Api.Controllers;

[ApiController]
[Route("api/admin/iam")]
[Authorize(Roles = "Admin")]
public sealed class OrganizationManagementController(IOrganizationManagementService service) : ControllerBase
{
    [HttpGet("organization-types")]
    public async Task<IActionResult> GetOrganizationTypes(CancellationToken ct) => Ok(await service.GetOrganizationTypesAsync(ct));
    [HttpPost("organization-types")]
    public async Task<IActionResult> CreateOrganizationType(CreateOrganizationTypeRequest request, CancellationToken ct) => Ok(await service.CreateOrganizationTypeAsync(request, ct));
    [HttpPut("organization-types/{id:int}")]
    public async Task<IActionResult> UpdateOrganizationType(int id, UpdateOrganizationTypeRequest request, CancellationToken ct) => Ok(await service.UpdateOrganizationTypeAsync(id, request, ct));
    [HttpPatch("organization-types/{id:int}/active")]
    public async Task<IActionResult> SetOrganizationTypeActive(int id, [FromQuery] bool value, CancellationToken ct) { await service.SetOrganizationTypeActiveAsync(id, value, ct); return NoContent(); }

    [HttpGet("organization-units")]
    public async Task<IActionResult> GetOrganizationUnits(CancellationToken ct) => Ok(await service.GetOrganizationUnitsAsync(ct));
    [HttpPost("organization-units")]
    public async Task<IActionResult> CreateOrganizationUnit(CreateOrganizationUnitRequest request, CancellationToken ct) => Ok(await service.CreateOrganizationUnitAsync(request, ct));
    [HttpPut("organization-units/{id:int}")]
    public async Task<IActionResult> UpdateOrganizationUnit(int id, UpdateOrganizationUnitRequest request, CancellationToken ct) => Ok(await service.UpdateOrganizationUnitAsync(id, request, ct));
    [HttpPatch("organization-units/{id:int}/active")]
    public async Task<IActionResult> SetOrganizationUnitActive(int id, [FromQuery] bool value, CancellationToken ct) { await service.SetOrganizationUnitActiveAsync(id, value, ct); return NoContent(); }

    [HttpGet("positions")]
    public async Task<IActionResult> GetPositions(CancellationToken ct) => Ok(await service.GetPositionsAsync(ct));
    [HttpPost("positions")]
    public async Task<IActionResult> CreatePosition(CreatePositionRequest request, CancellationToken ct) => Ok(await service.CreatePositionAsync(request, ct));
    [HttpPut("positions/{id:int}")]
    public async Task<IActionResult> UpdatePosition(int id, UpdatePositionRequest request, CancellationToken ct) => Ok(await service.UpdatePositionAsync(id, request, ct));
    [HttpPatch("positions/{id:int}/active")]
    public async Task<IActionResult> SetPositionActive(int id, [FromQuery] bool value, CancellationToken ct) { await service.SetPositionActiveAsync(id, value, ct); return NoContent(); }

    [HttpGet("genders")]
    public async Task<IActionResult> GetGenders(CancellationToken ct) => Ok(await service.GetGendersAsync(ct));
    [HttpPost("genders")]
    public async Task<IActionResult> CreateGender(CreateGenderRequest request, CancellationToken ct) => Ok(await service.CreateGenderAsync(request, ct));
    [HttpPut("genders/{id:int}")]
    public async Task<IActionResult> UpdateGender(int id, UpdateGenderRequest request, CancellationToken ct) => Ok(await service.UpdateGenderAsync(id, request, ct));

    [HttpGet("user-positions")]
    public async Task<IActionResult> GetUserPositions([FromQuery] Guid? userId, CancellationToken ct) => Ok(await service.GetUserPositionsAsync(userId, ct));
    [HttpPost("user-positions")]
    public async Task<IActionResult> CreateUserPosition(CreateUserPositionRequest request, CancellationToken ct) => Ok(await service.CreateUserPositionAsync(request, ct));
    [HttpPut("user-positions/{id:int}")]
    public async Task<IActionResult> UpdateUserPosition(int id, UpdateUserPositionRequest request, CancellationToken ct) => Ok(await service.UpdateUserPositionAsync(id, request, ct));
    [HttpPatch("user-positions/{id:int}/active")]
    public async Task<IActionResult> SetUserPositionActive(int id, [FromQuery] bool value, CancellationToken ct) { await service.SetUserPositionActiveAsync(id, value, ct); return NoContent(); }
}
