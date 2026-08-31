using Microsoft.EntityFrameworkCore;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class UserRepository(SsoDbContext context) : IUserRepository
{
    public async Task<SsoUser?> FindByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleOrDefaultAsync(
            x => x.NationalCodeValue == nationalCode, cancellationToken);
        return await AttachRoleAsync(user, cancellationToken);
    }

    public async Task<SsoUser?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await AttachRoleAsync(
            await context.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken),
            cancellationToken);

    public Task AddAsync(SsoUser user, CancellationToken cancellationToken) =>
        context.Users.AddAsync(user, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);

    private async Task<SsoUser?> AttachRoleAsync(SsoUser? user, CancellationToken cancellationToken)
    {
        if (user is null)
        {
            return null;
        }

        var title = await (
            from assignment in context.UserRoles
            join role in context.Roles on assignment.RoleId equals role.Id
            where assignment.UserId == user.Id && role.Title.ToLower() == "admin"
            select role.Title).FirstOrDefaultAsync(cancellationToken);

        if (title is not null)
        {
            user.SetRole(UserRole.Admin);
        }

        return user;
    }
}
