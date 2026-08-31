using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.Mapping;
using SSO_Irica.Application.Services;
using SSO_Irica.Application.Validators;

namespace SSO_Irica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSsoApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(UserProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
