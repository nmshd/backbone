using System.Reflection;
using Backbone.Modules.Synchronization.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using Synchronization.Jobs.SanityCheck.Extensions;
using Synchronization.Jobs.SanityCheck.Infrastructure.DataSource;
using Synchronization.Jobs.SanityCheck.Infrastructure.Reporter;

namespace Synchronization.Jobs.SanityCheck;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.Sources.Clear();
                var env = hostContext.HostingEnvironment;

                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    configuration.AddUserSecrets(appAssembly, optional: true);
                }

                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddHostedService<Worker>();

                services.AddScoped<IDataSource, DataSource>();

                services.AddScoped<IReporter, LogReporter>();

                services.AddPersistence(options =>
                {
                    options.DbOptions.DbConnectionString = configuration.GetSqlDatabaseConfiguration().ConnectionString;

                    options.BlobStorageOptions.ConnectionInfo = configuration.GetBlobStorageConfiguration().ConnectionString;
                    options.BlobStorageOptions.CloudProvider = configuration.GetBlobStorageConfiguration().CloudProvider;
                    options.BlobStorageOptions.Container = configuration.GetBlobStorageConfiguration().ContainerName.IsNullOrEmpty() ? "synchronization" : configuration.GetBlobStorageConfiguration().ContainerName;
                });
            });
    }
}