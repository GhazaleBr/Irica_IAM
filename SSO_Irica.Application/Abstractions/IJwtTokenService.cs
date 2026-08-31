using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Application.Abstractions;

public interface IJwtTokenService
{
    string Create(SsoUser user, IEnumerable<string>? accessClaims = null);
}
