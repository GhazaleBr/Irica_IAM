using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Infrastructure.Logging;

public sealed class ElasticsearchAuditQueryService(
    IHttpClientFactory clients,
    IOptions<ElasticsearchOptions> options) : IAuditQueryService
{
    public async Task<AuditQueryResponse> SearchAsync(AuditQueryRequest request, CancellationToken ct = default)
    {
        var page = Math.Clamp(request.Page, 1, 10_000);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var must = new List<object>();
        if (!string.IsNullOrWhiteSpace(request.EventName))
            must.Add(new { term = new Dictionary<string, string> { ["eventName.keyword"] = request.EventName.Trim() } });
        if (!string.IsNullOrWhiteSpace(request.UserId))
            must.Add(new { term = new Dictionary<string, string> { ["userId.keyword"] = request.UserId.Trim() } });
        if (request.From.HasValue || request.To.HasValue)
        {
            var range = new Dictionary<string, object>();
            if (request.From.HasValue) range["gte"] = request.From.Value;
            if (request.To.HasValue) range["lte"] = request.To.Value;
            must.Add(new { range = new Dictionary<string, object> { ["timestamp"] = range } });
        }

        var payload = new
        {
            from = (page - 1) * pageSize,
            size = pageSize,
            sort = new[] { new { timestamp = new { order = "desc" } } },
            query = new { @bool = new { must } }
        };

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Url))
            return new AuditQueryResponse(0, Array.Empty<AuditEventResponse>());

        var client = clients.CreateClient("elasticsearch");
        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            client.DefaultRequestHeaders.Authorization = new("ApiKey", settings.ApiKey);
        using var response = await client.PostAsJsonAsync(
            $"{settings.Url.TrimEnd('/')}/{settings.Index}/_search", payload, ct);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var root = document.RootElement;
        var total = root.GetProperty("hits").GetProperty("total");
        var totalValue = total.ValueKind == JsonValueKind.Object
            ? total.GetProperty("value").GetInt64()
            : total.GetInt64();
        var items = new List<AuditEventResponse>();
        foreach (var hit in root.GetProperty("hits").GetProperty("hits").EnumerateArray())
        {
            var source = hit.GetProperty("_source");
            items.Add(new AuditEventResponse(
                ReadDate(source, "timestamp"),
                ReadString(source, "eventName"),
                ReadString(source, "userId"),
                ReadString(source, "action"),
                ReadString(source, "resource"),
                source.TryGetProperty("statusCode", out var status) && status.TryGetInt32(out var code) ? code : 0,
                ReadString(source, "ipAddress")));
        }
        return new AuditQueryResponse(totalValue, items);
    }

    private static string? ReadString(JsonElement source, string name) =>
        source.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.GetString() : null;

    private static DateTimeOffset? ReadDate(JsonElement source, string name) =>
        source.TryGetProperty(name, out var value) && value.TryGetDateTimeOffset(out var result) ? result : null;
}
