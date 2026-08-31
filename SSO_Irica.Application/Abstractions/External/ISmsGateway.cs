namespace SSO_Irica.Application.Abstractions.External;

public interface ISmsGateway
{
    Task SendAsync(string mobile, string message, CancellationToken cancellationToken = default);
}
