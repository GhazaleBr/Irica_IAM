using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Application.Abstractions;

public interface IUserRepository
{
    Task<SsoUser?> FindByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
    Task<SsoUser?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(SsoUser user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
