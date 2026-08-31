using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class SsoDbContextFactory : IDesignTimeDbContextFactory<SsoDbContext>
{
    public SsoDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiDirectory = FindApiDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{environment}.json", true)
            .AddEnvironmentVariables()
            .Build();
        var connection = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("SSO ConnectionStrings:DefaultConnection is required.");
        return new SsoDbContext(new DbContextOptionsBuilder<SsoDbContext>()
            .UseNpgsql(connection)
            .Options);
    }

    private static string FindApiDirectory()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                var path = Path.Combine(directory.FullName, "SSO_Irica", "SSO_Irica.Api");
                if (File.Exists(Path.Combine(path, "appsettings.json"))) return path;
                if (File.Exists(Path.Combine(directory.FullName, "appsettings.json"))) return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not find SSO_Irica.Api/appsettings.json.");
    }
}
