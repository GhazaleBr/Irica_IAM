namespace SSO_Irica.Application.Abstractions.External;

public interface IIdentityVerificationClient
{
    Task<bool> VerifyAsync(string mobile, string nationalCode, CancellationToken cancellationToken = default);
}
