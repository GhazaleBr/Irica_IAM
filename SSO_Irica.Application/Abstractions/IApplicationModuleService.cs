using SSO_Irica.Application.DTOs.Access.Requests;
using SSO_Irica.Application.DTOs.Access.Responses;

namespace SSO_Irica.Application.Abstractions;

public interface IApplicationModuleService
{
    Task<IReadOnlyList<ApplicationResponse>> GetApplicationsAsync(CancellationToken ct = default);
    Task<ApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request, CancellationToken ct = default);
    Task<ApplicationResponse> UpdateApplicationAsync(int id, UpdateApplicationRequest request, CancellationToken ct = default);
    Task SetApplicationActiveAsync(int id, bool value, CancellationToken ct = default);
    Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(int? applicationId = null, CancellationToken ct = default);
    Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, CancellationToken ct = default);
    Task<ModuleResponse> UpdateModuleAsync(int id, UpdateModuleRequest request, CancellationToken ct = default);
}
