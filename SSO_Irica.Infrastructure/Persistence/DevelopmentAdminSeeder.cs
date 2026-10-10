using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Domain.Identity;
using SSO_Irica.Domain.Identity.ValueObjects;

namespace SSO_Irica.Infrastructure.Persistence;

public static class DevelopmentAdminSeeder
{
    public static async Task SeedDevelopmentAdminAsync(this IServiceProvider services,
        IHostEnvironment environment, IConfiguration configuration, CancellationToken ct = default)
    {
        if (!environment.IsDevelopment() || !configuration.GetValue<bool>("DevelopmentAdmin:Enabled")) return;
        var connection = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
        if (connection.Host is not ("localhost" or "127.0.0.1" or "::1"))
            throw new InvalidOperationException("Development admin seeding requires a local PostgreSQL server.");
        var nationalCode = NationalCode.Create(configuration["DevelopmentAdmin:NationalCode"] ?? "9876543210");
        var mobile = MobileNumber.Create(configuration["DevelopmentAdmin:Mobile"] ?? "09120000000");
        var credentialsPath = Path.Combine(environment.ContentRootPath, ".local", "development-admin.json");

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SsoDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await db.Database.MigrateAsync(ct);
        await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);
            await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(20261010, 1)", ct);
            await AlignSequencesAsync(db, ct);
            var user = await db.Users.SingleOrDefaultAsync(entry => entry.NationalCodeValue == nationalCode.Value, ct);
            string password;
            if (File.Exists(credentialsPath))
            {
                var credentials = JsonSerializer.Deserialize<DevelopmentCredentials>(await File.ReadAllTextAsync(credentialsPath, ct))
                    ?? throw new InvalidOperationException("Invalid local development credentials file.");
                if (credentials.NationalCode != nationalCode.Value)
                    throw new InvalidOperationException("DevelopmentAdmin:NationalCode does not match the local credentials file.");
                password = credentials.Password;
            }
            else
            {
                if (user is not null)
                    throw new InvalidOperationException("Development account already exists but its local credentials file is missing. Choose an unused DevelopmentAdmin:NationalCode; existing passwords are never reset.");
                password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
                Directory.CreateDirectory(Path.GetDirectoryName(credentialsPath)!);
                await using var stream = new FileStream(credentialsPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(stream, new DevelopmentCredentials(nationalCode.Value, password), cancellationToken: ct);
            }

            if (user is null)
            {
                user = SsoUser.Register(nationalCode, mobile, "Development Administrator", hasher.Hash(password));
                db.Users.Add(user);
            }
            else if (!user.IsActive || !hasher.Verify(password, user.PasswordHash))
                throw new InvalidOperationException("The development account is inactive or its password was changed. Automatic seeding will not reactivate or reset it.");

            var role = await db.Roles.FirstOrDefaultAsync(entry => entry.Title.ToLower() == "admin", ct);
            if (role is null)
            {
                role = new RoleRecord
                {
                    Code = Math.Max(2, (await db.Roles.MaxAsync(entry => (int?)entry.Code, ct) ?? 1) + 1),
                    Title = "Admin", Description = "Full administrator access", IsActive = true
                };
                db.Roles.Add(role);
            }
            role.IsActive = true;
            await db.SaveChangesAsync(ct);
            if (!await db.UserRoles.AnyAsync(entry => entry.UserId == user.Id && entry.RoleId == role.Id, ct))
                db.UserRoles.Add(new UserRoleRecord { UserId = user.Id, RoleId = role.Id });

            var iam = await EnsureApplicationAsync(db, "IAM", "Identity and access management", ct);
            var portal = await EnsureApplicationAsync(db, "PORTAL_ADMIN", "Portal administration", ct);
            foreach (var code in new[] { "IAM", "IAM_USERS", "IAM_ROLES", "IAM_ORGANIZATIONS", "IAM_UNITS", "IAM_POSITIONS", "IAM_APPLICATIONS", "IAM_MODULES" })
                await EnsureModuleAsync(db, iam.Id, code, ct);
            foreach (var code in new[] { "PORTAL_ADMIN", "PORTAL_CONTENT", "PORTAL_MEDIA", "PORTAL_FORMS", "PORTAL_PROVINCES", "PORTAL_REFERENCE_DATA" })
                await EnsureModuleAsync(db, portal.Id, code, ct);
            foreach (var code in new[] { "read", "create", "update", "delete", "manage", "portal.admin.write" })
            {
                if (!await db.Permissions.AnyAsync(entry => entry.Code == code, ct))
                    db.Permissions.Add(new PermissionRecord { Code = code, Title = code });
            }
            await db.SaveChangesAsync(ct);
            var modules = await db.Modules.ToListAsync(ct);
            var permissions = await db.Permissions.ToListAsync(ct);
            var grants = await db.RolePermissions.Where(entry => entry.RoleId == role.Id).ToListAsync(ct);
            foreach (var module in modules)
            foreach (var permission in permissions)
            {
                var grant = grants.FirstOrDefault(entry => entry.ModuleId == module.Id && entry.PermissionId == permission.Id);
                if (grant is null)
                    db.RolePermissions.Add(new RolePermissionRecord { RoleId = role.Id, ModuleId = module.Id, PermissionId = permission.Id, IsActive = true });
                else
                    grant.IsActive = true;
            }
            await EnsureOrganizationAsync(db, user.Id, ct);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        });
        services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DevelopmentAdminSeeder))
            .LogInformation("Development administrator is ready. Local credentials: {CredentialsPath}", credentialsPath);
    }

    private static async Task<ApplicationRecord> EnsureApplicationAsync(SsoDbContext db, string code, string title, CancellationToken ct)
    {
        var application = await db.Applications.SingleOrDefaultAsync(entry => entry.Code == code, ct);
        if (application is not null) return application;
        application = new ApplicationRecord { Code = code, Title = title, IsActive = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync(ct);
        return application;
    }

    private static async Task EnsureModuleAsync(SsoDbContext db, int applicationId, string code, CancellationToken ct)
    {
        if (await db.Modules.AnyAsync(entry => entry.Code == code, ct)) return;
        db.Modules.Add(new ModuleRecord { Code = code, Title = code, ApplicationId = applicationId });
        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsureOrganizationAsync(SsoDbContext db, Guid userId, CancellationToken ct)
    {
        const string title = "Development administration";
        var type = await db.OrganizationTypes.FirstOrDefaultAsync(entry => entry.Title == title, ct);
        if (type is null)
        {
            type = new OrganizationTypeRecord { Code = (await db.OrganizationTypes.MaxAsync(entry => (int?)entry.Code, ct) ?? 0) + 1, Title = title, IsActive = true };
            db.OrganizationTypes.Add(type);
            await db.SaveChangesAsync(ct);
        }
        var unit = await db.OrganizationUnits.FirstOrDefaultAsync(entry => entry.Title == title && entry.TypeId == type.Id, ct);
        if (unit is null)
        {
            unit = new OrganizationUnitRecord { Code = (await db.OrganizationUnits.MaxAsync(entry => (int?)entry.Code, ct) ?? 0) + 1, Title = title, TypeId = type.Id, IsActive = true };
            db.OrganizationUnits.Add(unit);
            await db.SaveChangesAsync(ct);
        }
        var position = await db.Positions.FirstOrDefaultAsync(entry => entry.Title == title && entry.OrganizationUnitId == unit.Id, ct);
        if (position is null)
        {
            position = new PositionRecord { Code = (await db.Positions.MaxAsync(entry => (int?)entry.Code, ct) ?? 0) + 1, Title = title, OrganizationUnitId = unit.Id, IsActive = true };
            db.Positions.Add(position);
            await db.SaveChangesAsync(ct);
        }
        if (!await db.UserPositions.AnyAsync(entry => entry.UserId == userId && entry.PositionId == position.Id, ct))
            db.UserPositions.Add(new UserPositionRecord
            {
                UserId = userId, PositionId = position.Id, IsActive = true,
                IsPrimary = !await db.UserPositions.AnyAsync(entry => entry.UserId == userId && entry.IsPrimary, ct),
                StartDateTime = DateTime.UtcNow
            });
    }

    private static async Task AlignSequencesAsync(SsoDbContext db, CancellationToken ct)
    {
        foreach (var table in new[] { "Tbl_Application", "Tbl_Roles", "Tbl_User_Roles", "Tbl_Modules", "Tbl_Permissions", "Tbl_Role_Permissions", "Tbl_Organization_Type", "Tbl_Organization_Unit", "Tbl_Positions", "Tbl_User_Positions" })
        {
            var sql = $"""
                LOCK TABLE "{table}" IN SHARE ROW EXCLUSIVE MODE;
                SELECT setval(pg_get_serial_sequence('"{table}"', 'Fld_Id'),
                    GREATEST(COALESCE((SELECT MAX("Fld_Id") FROM "{table}"), 0) + 1,
                        nextval(pg_get_serial_sequence('"{table}"', 'Fld_Id'))), false)
                """;
            await db.Database.ExecuteSqlRawAsync(sql, ct);
        }
    }

    private sealed record DevelopmentCredentials(string NationalCode, string Password);
}
