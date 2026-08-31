namespace SSO_Irica.Application.Abstractions;

public interface ISmsSender
{
    Task SendAsync(string mobile, string message, CancellationToken cancellationToken);
}
