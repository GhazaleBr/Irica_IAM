using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using SSO_Irica.Application.Abstractions;

namespace SSO_Irica.Infrastructure.Security;

public sealed class InMemoryOtpService(
    ISmsSender smsSender,
    IHostEnvironment environment) : IOtpService
{
    private const int MaxAttempts = 5;
    private static readonly ConcurrentDictionary<Guid, OtpEntry> Codes = new();
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

    public async Task<string?> SendAsync(Guid userId, string mobile, CancellationToken cancellationToken)
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        Codes[userId] = new OtpEntry(code, DateTimeOffset.UtcNow.Add(Lifetime));
        await smsSender.SendAsync(mobile, $"Your SSO verification code is: {code}", cancellationToken);
        return environment.IsDevelopment() ? code : null;
    }

    public Task<bool> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        if (!Codes.TryGetValue(userId, out var stored) || stored.Expires < DateTimeOffset.UtcNow)
            return Task.FromResult(false);
        var matched = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(stored.Code), Encoding.UTF8.GetBytes(code));
        if (matched || Interlocked.Increment(ref stored.Attempts) >= MaxAttempts)
        {
            Codes.TryRemove(userId, out _);
        }
        return Task.FromResult(matched);
    }

    private sealed class OtpEntry(string code, DateTimeOffset expires)
    {
        public string Code { get; } = code;
        public DateTimeOffset Expires { get; } = expires;
        public int Attempts;
    }
}
