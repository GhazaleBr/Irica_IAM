global using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PortalAdmin.Api.Setup;
using PortalAdmin.Api.Controllers;
using PortalAdmin.Infrastructure.Mutations;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Domain.Identity;
using SSO_Irica.Domain.Identity.ValueObjects;
using SSO_Irica.Infrastructure;
using SSO_Irica.Infrastructure.Persistence;

var root = Directory.GetCurrentDirectory();
var api = Path.Combine(root, "SSO_Irica.Api");
var configuration = new ConfigurationBuilder().SetBasePath(api)
    .AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json").Build();
var portalConfiguration = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(Path.Combine(root, "../Portal_Admin_Back/src/PortalAdmin.Api")))
    .AddJsonFile("appsettings.Development.json").Build();
var services = new ServiceCollection();
services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Error));
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<IHostEnvironment>(new VerificationEnvironment { ContentRootPath = api });
services.AddSsoInfrastructure(configuration);
services.AddAdminWritePolicy();
await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<SsoDbContext>();
async Task<string> Counts() => string.Join(",", await db.Users.CountAsync(), await db.Roles.CountAsync(),
    await db.UserRoles.CountAsync(), await db.Modules.CountAsync(), await db.RolePermissions.CountAsync(),
    await db.OrganizationTypes.CountAsync(), await db.OrganizationUnits.CountAsync(), await db.Positions.CountAsync(), await db.UserPositions.CountAsync());
var before = await Counts();
await provider.SeedDevelopmentAdminAsync(provider.GetRequiredService<IHostEnvironment>(), configuration);
if (before != await Counts()) throw new Exception("Seed is not idempotent.");
var user = await scope.ServiceProvider.GetRequiredService<IUserRepository>().FindByNationalCodeAsync(configuration["DevelopmentAdmin:NationalCode"]!, default)
    ?? throw new Exception("Missing developer account.");
if (user.Role != UserRole.Admin) throw new Exception("Missing admin role.");
var issuer = provider.GetRequiredService<IJwtTokenService>();
var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
System.Security.Claims.ClaimsPrincipal Validate(string token) => handler.ValidateToken(token, new TokenValidationParameters
{
    ValidateIssuer = true, ValidIssuer = portalConfiguration["Iam:Issuer"],
    ValidateAudience = true, ValidAudience = portalConfiguration["Iam:Audience"],
    ValidateLifetime = true, ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(portalConfiguration["Iam:SigningKey"]!)),
    NameClaimType = "sub", RoleClaimType = "role"
}, out _);
var principal = Validate(issuer.Create(user));
var authorization = provider.GetRequiredService<IAuthorizationService>();
if (!(await authorization.AuthorizeAsync(principal, null, AuthorizationSetup.AdminWritePolicy)).Succeeded)
    throw new Exception("Portal rejected IAM administrator.");
var actor = AdminActorResolver.From(principal);
if (actor.ProvinceId is not null || actor.Id != user.Id.ToString()) throw new Exception("Unexpected actor scope.");
ProvinceScopeGuard.Check(new { ProvinceId = 2 }, actor, false);
ProvinceScopeGuard.Check(new { ProvinceId = 30 }, actor, false);
ProvinceScopeGuard.Check(new { Title = "Organization" }, actor, true);
var ordinary = SsoUser.Register(NationalCode.Create("1234567891"), MobileNumber.Create("09120000000"), "Test User", "test-hash");
if ((await authorization.AuthorizeAsync(Validate(issuer.Create(ordinary)), null, AuthorizationSetup.AdminWritePolicy)).Succeeded)
    throw new Exception("Ordinary user unexpectedly has admin access.");
await provider.SeedDevelopmentAdminAsync(new VerificationEnvironment { EnvironmentName = Environments.Production }, new ConfigurationBuilder().Build());
Console.WriteLine("PASS: idempotent seed; real database admin role; IAM JWT accepted by Portal; central access across provinces/resources; ordinary user denied; Production seed disabled.");

sealed class VerificationEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = Environments.Development;
    public string ApplicationName { get; set; } = "VerifyAdmin";
    public string ContentRootPath { get; set; } = "";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
