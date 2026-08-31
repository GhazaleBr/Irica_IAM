using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Api.Controllers;

[ApiController]
[Route("api/admin/audit")]
[Authorize(Roles = "Admin")]
public sealed class AuditController(IAuditQueryService audit) : ControllerBase
{
    [HttpGet("events")]
    public async Task<ActionResult<AuditQueryResponse>> Search([FromQuery] AuditQueryRequest request, CancellationToken ct) =>
        Ok(await audit.SearchAsync(request, ct));
}
