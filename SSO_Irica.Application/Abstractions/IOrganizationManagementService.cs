using SSO_Irica.Application.DTOs.Organization.Requests;
using SSO_Irica.Application.DTOs.Organization.Responses;

namespace SSO_Irica.Application.Abstractions;

public interface IOrganizationManagementService
{
    Task<IReadOnlyList<OrganizationTypeResponse>> GetOrganizationTypesAsync(CancellationToken ct = default);
    Task<OrganizationTypeResponse> CreateOrganizationTypeAsync(CreateOrganizationTypeRequest request, CancellationToken ct = default);
    Task<OrganizationTypeResponse> UpdateOrganizationTypeAsync(int id, UpdateOrganizationTypeRequest request, CancellationToken ct = default);
    Task SetOrganizationTypeActiveAsync(int id, bool isActive, CancellationToken ct = default);

    Task<IReadOnlyList<OrganizationUnitResponse>> GetOrganizationUnitsAsync(CancellationToken ct = default);
    Task<OrganizationUnitResponse> CreateOrganizationUnitAsync(CreateOrganizationUnitRequest request, CancellationToken ct = default);
    Task<OrganizationUnitResponse> UpdateOrganizationUnitAsync(int id, UpdateOrganizationUnitRequest request, CancellationToken ct = default);
    Task SetOrganizationUnitActiveAsync(int id, bool isActive, CancellationToken ct = default);

    Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(CancellationToken ct = default);
    Task<PositionResponse> CreatePositionAsync(CreatePositionRequest request, CancellationToken ct = default);
    Task<PositionResponse> UpdatePositionAsync(int id, UpdatePositionRequest request, CancellationToken ct = default);
    Task SetPositionActiveAsync(int id, bool isActive, CancellationToken ct = default);

    Task<IReadOnlyList<GenderResponse>> GetGendersAsync(CancellationToken ct = default);
    Task<GenderResponse> CreateGenderAsync(CreateGenderRequest request, CancellationToken ct = default);
    Task<GenderResponse> UpdateGenderAsync(int id, UpdateGenderRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<UserPositionResponse>> GetUserPositionsAsync(Guid? userId = null, CancellationToken ct = default);
    Task<UserPositionResponse> CreateUserPositionAsync(CreateUserPositionRequest request, CancellationToken ct = default);
    Task<UserPositionResponse> UpdateUserPositionAsync(int id, UpdateUserPositionRequest request, CancellationToken ct = default);
    Task SetUserPositionActiveAsync(int id, bool isActive, CancellationToken ct = default);
}
