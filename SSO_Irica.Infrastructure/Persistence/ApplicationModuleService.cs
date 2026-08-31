using Microsoft.EntityFrameworkCore;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Access.Requests;
using SSO_Irica.Application.DTOs.Access.Responses;
using SSO_Irica.Application.Exceptions;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class ApplicationModuleService(SsoDbContext db) : IApplicationModuleService
{
    public async Task<IReadOnlyList<ApplicationResponse>> GetApplicationsAsync(CancellationToken ct = default) =>
        await db.Applications.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new ApplicationResponse(x.Id, x.Code, x.Title, x.IsActive)).ToListAsync(ct);

    public async Task<ApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request, CancellationToken ct = default)
    {
        EnsureUnique(await db.Applications.AnyAsync(x => x.Code == request.Code, ct));
        var entity = new ApplicationRecord { Code = request.Code.Trim(), Title = request.Title.Trim(), IsActive = true };
        db.Applications.Add(entity); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.IsActive);
    }

    public async Task<ApplicationResponse> UpdateApplicationAsync(int id, UpdateApplicationRequest request, CancellationToken ct = default)
    {
        var entity = await db.Applications.FindAsync([id], ct) ?? throw NotFound(ErrorCatalog.ApplicationNotFound, "Application was not found.");
        EnsureUnique(await db.Applications.AnyAsync(x => x.Code == request.Code && x.Id != id, ct));
        entity.Code = request.Code.Trim(); entity.Title = request.Title.Trim(); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.IsActive);
    }

    public Task SetApplicationActiveAsync(int id, bool value, CancellationToken ct = default) =>
        SetActiveAsync(db.Applications, id, value, ErrorCatalog.ApplicationNotFound, ct);

    public async Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(int? applicationId = null, CancellationToken ct = default)
    {
        var query = db.Modules.AsNoTracking().AsQueryable();
        if (applicationId.HasValue) query = query.Where(x => x.ApplicationId == applicationId.Value);
        return await query.OrderBy(x => x.Title)
            .Select(x => new ModuleResponse(x.Id, x.Code, x.Title, x.ApplicationId)).ToListAsync(ct);
    }

    public async Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, CancellationToken ct = default)
    {
        await EnsureApplicationAsync(request.ApplicationId, ct);
        EnsureUnique(await db.Modules.AnyAsync(x => x.Code == request.Code, ct));
        var entity = new ModuleRecord { Code = request.Code.Trim(), Title = request.Title.Trim(), ApplicationId = request.ApplicationId };
        db.Modules.Add(entity); await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.ApplicationId);
    }

    public async Task<ModuleResponse> UpdateModuleAsync(int id, UpdateModuleRequest request, CancellationToken ct = default)
    {
        var entity = await db.Modules.FindAsync([id], ct) ?? throw NotFound(ErrorCatalog.OrganizationNotFound, "Module was not found.");
        await EnsureApplicationAsync(request.ApplicationId, ct);
        EnsureUnique(await db.Modules.AnyAsync(x => x.Code == request.Code && x.Id != id, ct));
        entity.Code = request.Code.Trim(); entity.Title = request.Title.Trim(); entity.ApplicationId = request.ApplicationId; await db.SaveChangesAsync(ct);
        return new(entity.Id, entity.Code, entity.Title, entity.ApplicationId);
    }

    private async Task EnsureApplicationAsync(int id, CancellationToken ct)
    {
        if (!await db.Applications.AnyAsync(x => x.Id == id && x.IsActive, ct))
            throw NotFound(ErrorCatalog.ApplicationNotFound, "Application was not found or is inactive.");
    }

    private static void EnsureUnique(bool exists)
    {
        if (exists) throw new AppException(ErrorCatalog.DuplicateCatalogCode, "The code already exists.", 409);
    }

    private static AppException NotFound(string code, string message) => new(code, message, 404);

    private async Task SetActiveAsync<TEntity>(DbSet<TEntity> set, int id, bool value, string code, CancellationToken ct)
        where TEntity : class
    {
        var entity = await set.FindAsync([id], ct) ?? throw NotFound(code, "Record was not found.");
        switch (entity)
        {
            case ApplicationRecord application: application.IsActive = value; break;
            default: throw new InvalidOperationException("Active state is not supported.");
        }
        await db.SaveChangesAsync(ct);
    }
}
