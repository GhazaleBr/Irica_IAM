namespace SSO_Irica.Application.Abstractions;

public interface IOtpService
{
    Task<string?> SendAsync(Guid userId, string mobile, CancellationToken cancellationToken);
    Task<bool> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken);
}
