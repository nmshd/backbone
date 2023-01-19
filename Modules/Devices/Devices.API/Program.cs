using Devices.API.ExtensionMethods;
using Devices.API.Extensions;
using Devices.Infrastructure.Persistence.Database;
using Enmeshed.Tooling.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Serilog;
using System.Reflection;

namespace Devices.API;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        CreateWebHostBuilder(args).Build()
           .MigrateDbContext<ApplicationDbContext>((context, _) => { new ApplicationDbContextSeed().SeedAsync(context).Wait(); })
           .MigrateDbContext<ConfigurationDbContext>((context, _) => { new ConfigurationDbContextSeed().SeedAsync(context).Wait(); })
           .Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.AddServerHeader = false;
                options.Limits.MaxRequestBodySize = 1.Kibibytes();
            })
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();
                IWebHostEnvironment env = hostingContext.HostingEnvironment;

                configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    Assembly appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        configuration.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                configuration.AddEnvironmentVariables();

                if (args != null)
                {
                    configuration.AddCommandLine(args);
                }

                configuration.AddAzureAppConfiguration(hostingContext);
            })
            .UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            })
            .UseStartup<Startup>();
    }
}
