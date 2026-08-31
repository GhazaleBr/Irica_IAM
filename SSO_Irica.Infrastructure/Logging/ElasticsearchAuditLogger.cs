using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions;

namespace SSO_Irica.Infrastructure.Logging;

public sealed class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";
    public string Url { get; init; } = string.Empty;
    public string Index { get; init; } = "sso-audit";
    public string ApiKey { get; init; } = string.Empty;
}

public sealed class ElasticsearchAuditLogger(
    IHttpClientFactory clients,
    IOptions<ElasticsearchOptions> options,
    ILogger<ElasticsearchAuditLogger> logger) : IAuditLogger
{
    public async Task WriteAsync(string eventName, string? userId, string? action, string? resource,
        int statusCode, string? ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = options.Value;
            if (string.IsNullOrWhiteSpace(settings.Url))
            {
                logger.LogInformation("Audit {EventName} user={UserId} action={Action} resource={Resource} status={StatusCode}",
                    eventName, userId, action, resource, statusCode);
                return;
            }
            var client = clients.CreateClient("elasticsearch");
            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
                client.DefaultRequestHeaders.Authorization = new("ApiKey", settings.ApiKey);
            using var response = await client.PostAsJsonAsync(
                $"{settings.Url.TrimEnd('/')}/{settings.Index}/_doc",
                new { timestamp = DateTimeOffset.UtcNow, eventName, userId, action, resource, statusCode, ipAddress },
                cancellationToken);
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("Elasticsearch audit failed with status {StatusCode}", response.StatusCode);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Audit sink unavailable; request processing continues.");
        }
    }
}
