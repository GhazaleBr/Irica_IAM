using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions.External;

namespace SSO_Irica.Infrastructure.ExternalServices;

public sealed class HttpSmsGateway(
    IHttpClientFactory clients,
    IOptions<ExternalApiOptions> options) : ISmsGateway
{
    public async Task SendAsync(string mobile, string message, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.SmsBaseUrl)) return;
        using var response = await clients.CreateClient("sms")
            .PostAsJsonAsync(settings.SmsPath, new { mobile, message }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
