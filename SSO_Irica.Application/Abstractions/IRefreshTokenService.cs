namespace SSO_Irica.Application.Abstractions;

public interface IRefreshTokenService
{
    Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken);
    Task<(Guid UserId, string NewRefreshToken)?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
