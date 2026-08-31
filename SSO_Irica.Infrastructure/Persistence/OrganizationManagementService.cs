using Microsoft.EntityFrameworkCore;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Organization.Requests;
using SSO_Irica.Application.DTOs.Organization.Responses;
using SSO_Irica.Application.Exceptions;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class OrganizationManagementService(SsoDbContext db) : IOrganizationManagementService
{
    public async Task<IReadOnlyList<OrganizationTypeResponse>> GetOrganizationTypesAsync(CancellationToken ct = default) =>
        await db.OrganizationTypes.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new OrganizationTypeResponse(x.Id, x.Code, x.Title, x.IsActive)).ToListAsync(ct);

    public async Task<OrganizationTypeResponse> CreateOrganizationTypeAsync(CreateOrganizationTypeRequest request, CancellationToken ct = default)
    {
        EnsureUnique(await db.OrganizationTypes.AnyAsync(x => x.Code == request.Code, ct), ErrorCatalog.DuplicateCatalogCode);
        var entity = new OrganizationTypeRecord { Code = request.Code, Title = request.Title.Trim(), IsActive = true };
        db.OrganizationTypes.Add(entity); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.IsActive);
    }

    public async Task<OrganizationTypeResponse> UpdateOrganizationTypeAsync(int id, UpdateOrganizationTypeRequest request, CancellationToken ct = default)
    {
        var entity = await db.OrganizationTypes.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw NotFound(ErrorCatalog.OrganizationTypeNotFound, "Organization type was not found.");
        EnsureUnique(await db.OrganizationTypes.AnyAsync(x => x.Code == request.Code && x.Id != id, ct), ErrorCatalog.DuplicateCatalogCode);
        entity.Code = request.Code; entity.Title = request.Title.Trim(); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.IsActive);
    }

    public Task SetOrganizationTypeActiveAsync(int id, bool isActive, CancellationToken ct = default) =>
        SetActiveAsync(db.OrganizationTypes, id, isActive, ErrorCatalog.OrganizationTypeNotFound, (x, value) => x.IsActive = value, ct);

    public async Task<IReadOnlyList<OrganizationUnitResponse>> GetOrganizationUnitsAsync(CancellationToken ct = default) =>
        await db.OrganizationUnits.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new OrganizationUnitResponse(x.Id, x.Code, x.Title, x.TypeId, x.ParentId, x.IsActive)).ToListAsync(ct);

    public async Task<OrganizationUnitResponse> CreateOrganizationUnitAsync(CreateOrganizationUnitRequest request, CancellationToken ct = default)
    {
        await EnsureOrganizationTypeAsync(request.TypeId, ct);
        await EnsureParentAsync(request.ParentId, ct);
        EnsureUnique(await db.OrganizationUnits.AnyAsync(x => x.Code == request.Code, ct), ErrorCatalog.DuplicateCatalogCode);
        var entity = new OrganizationUnitRecord { Code = request.Code, Title = request.Title.Trim(), TypeId = request.TypeId, ParentId = request.ParentId, IsActive = true };
        db.OrganizationUnits.Add(entity); await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<OrganizationUnitResponse> UpdateOrganizationUnitAsync(int id, UpdateOrganizationUnitRequest request, CancellationToken ct = default)
    {
        var entity = await db.OrganizationUnits.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw NotFound(ErrorCatalog.OrganizationNotFound, "Organization unit was not found.");
        if (request.ParentId == id) throw new AppException(ErrorCatalog.InvalidRelationship, "An organization unit cannot be its own parent.", 400);
        await EnsureOrganizationTypeAsync(request.TypeId, ct); await EnsureParentAsync(request.ParentId, ct);
        EnsureUnique(await db.OrganizationUnits.AnyAsync(x => x.Code == request.Code && x.Id != id, ct), ErrorCatalog.DuplicateCatalogCode);
        entity.Code = request.Code; entity.Title = request.Title.Trim(); entity.TypeId = request.TypeId; entity.ParentId = request.ParentId; await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public Task SetOrganizationUnitActiveAsync(int id, bool isActive, CancellationToken ct = default) =>
        SetActiveAsync(db.OrganizationUnits, id, isActive, ErrorCatalog.OrganizationNotFound, (x, value) => x.IsActive = value, ct);

    public async Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(CancellationToken ct = default) =>
        await db.Positions.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new PositionResponse(x.Id, x.Code, x.Title, x.OrganizationUnitId, x.Description, x.IsActive)).ToListAsync(ct);

    public async Task<PositionResponse> CreatePositionAsync(CreatePositionRequest request, CancellationToken ct = default)
    {
        await EnsureOrganizationUnitAsync(request.OrganizationUnitId, ct);
        EnsureUnique(await db.Positions.AnyAsync(x => x.Code == request.Code, ct), ErrorCatalog.DuplicateCatalogCode);
        var entity = new PositionRecord { Code = request.Code, Title = request.Title.Trim(), OrganizationUnitId = request.OrganizationUnitId, Description = request.Description?.Trim(), IsActive = true };
        db.Positions.Add(entity); await db.SaveChangesAsync(ct); return ToResponse(entity);
    }

    public async Task<PositionResponse> UpdatePositionAsync(int id, UpdatePositionRequest request, CancellationToken ct = default)
    {
        var entity = await db.Positions.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw NotFound(ErrorCatalog.PositionNotFound, "Position was not found.");
        await EnsureOrganizationUnitAsync(request.OrganizationUnitId, ct);
        EnsureUnique(await db.Positions.AnyAsync(x => x.Code == request.Code && x.Id != id, ct), ErrorCatalog.DuplicateCatalogCode);
        entity.Code = request.Code; entity.Title = request.Title.Trim(); entity.OrganizationUnitId = request.OrganizationUnitId; entity.Description = request.Description?.Trim(); await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public Task SetPositionActiveAsync(int id, bool isActive, CancellationToken ct = default) =>
        SetActiveAsync(db.Positions, id, isActive, ErrorCatalog.PositionNotFound, (x, value) => x.IsActive = value, ct);

    public async Task<IReadOnlyList<GenderResponse>> GetGendersAsync(CancellationToken ct = default) =>
        await db.Genders.AsNoTracking().OrderBy(x => x.Title).Select(x => new GenderResponse(x.Id, x.Code, x.Title)).ToListAsync(ct);

    public async Task<GenderResponse> CreateGenderAsync(CreateGenderRequest request, CancellationToken ct = default)
    {
        var entity = new GenderRecord { Code = request.Code, Title = request.Title.Trim() }; db.Genders.Add(entity); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title);
    }

    public async Task<GenderResponse> UpdateGenderAsync(int id, UpdateGenderRequest request, CancellationToken ct = default)
    {
        var entity = await db.Genders.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw NotFound(ErrorCatalog.GenderNotFound, "Gender was not found.");
        entity.Code = request.Code; entity.Title = request.Title.Trim(); await db.SaveChangesAsync(ct); return new(entity.Id, entity.Code, entity.Title);
    }

    public async Task<IReadOnlyList<UserPositionResponse>> GetUserPositionsAsync(Guid? userId = null, CancellationToken ct = default)
    {
        var query = db.UserPositions.AsNoTracking().AsQueryable();
        if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
        return await query.OrderByDescending(x => x.IsPrimary).ThenBy(x => x.StartDateTime)
            .Select(x => new UserPositionResponse(x.Id, x.UserId, x.PositionId, x.IsPrimary, x.StartDateTime, x.EndDateTime, x.IsActive)).ToListAsync(ct);
    }

    public async Task<UserPositionResponse> CreateUserPositionAsync(CreateUserPositionRequest request, CancellationToken ct = default)
    {
        await EnsureUserAsync(request.UserId, ct); await EnsurePositionAsync(request.PositionId, ct);
        var entity = new UserPositionRecord { UserId = request.UserId, PositionId = request.PositionId, IsPrimary = request.IsPrimary, StartDateTime = request.StartDateTime, EndDateTime = request.EndDateTime, IsActive = true };
        db.UserPositions.Add(entity); await db.SaveChangesAsync(ct); return ToResponse(entity);
    }

    public async Task<UserPositionResponse> UpdateUserPositionAsync(int id, UpdateUserPositionRequest request, CancellationToken ct = default)
    {
        var entity = await db.UserPositions.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw NotFound(ErrorCatalog.PositionNotFound, "User position was not found.");
        await EnsurePositionAsync(request.PositionId, ct);
        entity.PositionId = request.PositionId; entity.IsPrimary = request.IsPrimary; entity.StartDateTime = request.StartDateTime; entity.EndDateTime = request.EndDateTime; await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public Task SetUserPositionActiveAsync(int id, bool isActive, CancellationToken ct = default) =>
        SetActiveAsync(db.UserPositions, id, isActive, ErrorCatalog.PositionNotFound, (x, value) => x.IsActive = value, ct);

    private async Task EnsureUserAsync(Guid id, CancellationToken ct) { if (!await db.Users.AnyAsync(x => x.Id == id, ct)) throw NotFound(ErrorCatalog.UserNotFound, "User was not found."); }
    private async Task EnsurePositionAsync(int id, CancellationToken ct) { if (!await db.Positions.AnyAsync(x => x.Id == id, ct)) throw NotFound(ErrorCatalog.PositionNotFound, "Position was not found."); }
    private async Task EnsureOrganizationUnitAsync(int id, CancellationToken ct) { if (!await db.OrganizationUnits.AnyAsync(x => x.Id == id, ct)) throw NotFound(ErrorCatalog.OrganizationNotFound, "Organization unit was not found."); }
    private async Task EnsureOrganizationTypeAsync(int id, CancellationToken ct) { if (!await db.OrganizationTypes.AnyAsync(x => x.Id == id, ct)) throw NotFound(ErrorCatalog.OrganizationTypeNotFound, "Organization type was not found."); }
    private async Task EnsureParentAsync(int? id, CancellationToken ct) { if (id.HasValue) await EnsureOrganizationUnitAsync(id.Value, ct); }
    private static void EnsureUnique(bool exists, string code) { if (exists) throw new AppException(code, "The code already exists.", 409); }
    private static AppException NotFound(string code, string message) => new(code, message, 404);
    private static OrganizationUnitResponse ToResponse(OrganizationUnitRecord x) => new(x.Id, x.Code, x.Title, x.TypeId, x.ParentId, x.IsActive);
    private static PositionResponse ToResponse(PositionRecord x) => new(x.Id, x.Code, x.Title, x.OrganizationUnitId, x.Description, x.IsActive);
    private static UserPositionResponse ToResponse(UserPositionRecord x) => new(x.Id, x.UserId, x.PositionId, x.IsPrimary, x.StartDateTime, x.EndDateTime, x.IsActive);

    private async Task SetActiveAsync<TEntity>(DbSet<TEntity> set, int id, bool isActive, string errorCode, Action<TEntity, bool> setter, CancellationToken ct)
        where TEntity : class
    {
        var entity = await set.FindAsync([id], ct) ?? throw NotFound(errorCode, "The requested record was not found.");
        setter(entity, isActive);
        await db.SaveChangesAsync(ct);
    }
}
