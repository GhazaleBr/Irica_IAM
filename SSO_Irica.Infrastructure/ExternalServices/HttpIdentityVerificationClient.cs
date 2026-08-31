using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions.External;

namespace SSO_Irica.Infrastructure.ExternalServices;

public sealed class HttpIdentityVerificationClient(
    IHttpClientFactory clients,
    IOptions<ExternalApiOptions> options) : IIdentityVerificationClient
{
    public async Task<bool> VerifyAsync(string mobile, string nationalCode, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.IdentityVerificationBaseUrl)) return true;
        using var response = await clients.CreateClient("identity-verification")
            .PostAsJsonAsync(settings.IdentityVerificationPath, new { mobile, nationalCode }, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
