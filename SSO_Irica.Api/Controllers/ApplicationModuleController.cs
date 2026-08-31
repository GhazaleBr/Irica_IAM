using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Access.Requests;

namespace SSO_Irica.Api.Controllers;

[ApiController]
[Route("api/admin/access")]
[Authorize(Roles = "Admin")]
public sealed class ApplicationModuleController(IApplicationModuleService service) : ControllerBase
{
    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications(CancellationToken ct) => Ok(await service.GetApplicationsAsync(ct));

    [HttpPost("applications")]
    public async Task<IActionResult> CreateApplication(CreateApplicationRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await service.CreateApplicationAsync(request, ct));

    [HttpPut("applications/{id:int}")]
    public async Task<IActionResult> UpdateApplication(int id, UpdateApplicationRequest request, CancellationToken ct) =>
        Ok(await service.UpdateApplicationAsync(id, request, ct));

    [HttpPatch("applications/{id:int}/active")]
    public async Task<IActionResult> SetApplicationActive(int id, [FromQuery] bool value, CancellationToken ct)
    {
        await service.SetApplicationActiveAsync(id, value, ct);
        return NoContent();
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules([FromQuery] int? applicationId, CancellationToken ct) =>
        Ok(await service.GetModulesAsync(applicationId, ct));

    [HttpPost("modules")]
    public async Task<IActionResult> CreateModule(CreateModuleRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await service.CreateModuleAsync(request, ct));

    [HttpPut("modules/{id:int}")]
    public async Task<IActionResult> UpdateModule(int id, UpdateModuleRequest request, CancellationToken ct) =>
        Ok(await service.UpdateModuleAsync(id, request, ct));
}
