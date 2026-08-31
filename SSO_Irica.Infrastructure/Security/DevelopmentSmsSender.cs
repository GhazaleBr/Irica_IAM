using Microsoft.Extensions.Logging;
using SSO_Irica.Application.Abstractions;

namespace SSO_Irica.Infrastructure.Security;

public sealed class DevelopmentSmsSender(ILogger<DevelopmentSmsSender> logger) : ISmsSender
{
    public Task SendAsync(string mobile, string message, CancellationToken cancellationToken)
    {
        logger.LogWarning("Development SMS to {Mobile}: {Message}", mobile, message);
        return Task.CompletedTask;
    }
}
