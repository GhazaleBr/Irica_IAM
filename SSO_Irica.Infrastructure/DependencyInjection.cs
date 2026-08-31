using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Infrastructure.Persistence;
using SSO_Irica.Infrastructure.Security;
using SSO_Irica.Infrastructure.ExternalServices;
using SSO_Irica.Infrastructure.Logging;

namespace SSO_Irica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSsoInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<SsoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsql =>
            {
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                npgsql.CommandTimeout(30);
            })
            .EnableDetailedErrors(false));

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Jwt:Issuer is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Jwt:Audience is required.")
            .Validate(x => x.Key.Length >= 32, "Jwt:Key must contain at least 32 characters.")
            .ValidateOnStart();
        services.AddOptions<SsoSecurityOptions>()
            .BindConfiguration(SsoSecurityOptions.SectionName)
            .Validate(x => x.RefreshTokenDays is >= 1 and <= 30,
                "SsoSecurity:RefreshTokenDays must be between 1 and 30.")
            .ValidateOnStart();
        services.Configure<ExternalApiOptions>(configuration.GetSection(ExternalApiOptions.SectionName));
        services.Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));
        services.AddHttpClient("identity-verification", client =>
        {
            var url = configuration[$"{ExternalApiOptions.SectionName}:IdentityVerificationBaseUrl"];
            if (!string.IsNullOrWhiteSpace(url)) client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddHttpClient("sms", client =>
        {
            var url = configuration[$"{ExternalApiOptions.SectionName}:SmsBaseUrl"];
            if (!string.IsNullOrWhiteSpace(url)) client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddHttpClient("elasticsearch", client => client.Timeout = TimeSpan.FromSeconds(3));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleAccessService, RoleAccessService>();
        services.AddScoped<IOrganizationManagementService, OrganizationManagementService>();
        services.AddScoped<IApplicationModuleService, ApplicationModuleService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ISmsSender, DevelopmentSmsSender>();
        services.AddSingleton<IOtpService, InMemoryOtpService>();
        services.AddSingleton<IRefreshTokenService, InMemoryRefreshTokenService>();
        services.AddScoped<SSO_Irica.Application.Abstractions.External.IIdentityVerificationClient, HttpIdentityVerificationClient>();
        services.AddScoped<SSO_Irica.Application.Abstractions.External.ISmsGateway, HttpSmsGateway>();
        services.AddScoped<IAuditLogger, ElasticsearchAuditLogger>();
        services.AddScoped<IAuditQueryService, ElasticsearchAuditQueryService>();
        return services;
    }
}
