using System.Collections.Concurrent;
using System.Security.Cryptography;
using SSO_Irica.Application.Abstractions;

namespace SSO_Irica.Infrastructure.Security;

/// <summary>
/// Temporary refresh-token store. It keeps the IAM ERD free of token tables;
/// replace with a distributed cache in production.
/// </summary>
public sealed class InMemoryRefreshTokenService : IRefreshTokenService
{
    private readonly ConcurrentDictionary<string, (Guid UserId, DateTimeOffset ExpiresAt)> tokens = new();

    public Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        tokens[token] = (userId, DateTimeOffset.UtcNow.AddDays(7));
        return Task.FromResult(token);
    }

    public async Task<(Guid UserId, string NewRefreshToken)?> RotateAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (!tokens.TryRemove(refreshToken, out var current) || current.ExpiresAt <= DateTimeOffset.UtcNow)
            return null;
        var next = await IssueAsync(current.UserId, cancellationToken);
        return (current.UserId, next);
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        tokens.TryRemove(refreshToken, out _);
        return Task.CompletedTask;
    }
}
