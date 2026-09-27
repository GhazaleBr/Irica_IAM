using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Audit;

namespace SSO_Irica.Infrastructure.Logging;

public sealed class KafkaAuditOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; init; } = string.Empty;
    public string Topic { get; init; } = "iam-audit";
}

public sealed class KafkaAuditLogger(
    IProducer<string, string> producer,
    IOptions<KafkaAuditOptions> options,
    ILogger<KafkaAuditLogger> logger) : IAuditLogger
{
    public async Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(auditEvent, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        try
        {
            await producer.ProduceAsync(options.Value.Topic, new Message<string, string>
            {
                Key = auditEvent.UserId ?? auditEvent.EntityKey ?? auditEvent.Resource ?? auditEvent.EventName,
                Value = payload
            }, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to publish audit event {EventName} for {Action} to Kafka",
                auditEvent.EventName, auditEvent.Action);
        }
    }
}
