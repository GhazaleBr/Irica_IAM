using SSO_Irica.Application.DTOs.Auth.Requests;
using SSO_Irica.Application.DTOs.Auth.Responses;

namespace SSO_Irica.Application.Abstractions;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<TwoFactorChallengeResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthSession> VerifyTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken);
    Task<AuthSession?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
}
