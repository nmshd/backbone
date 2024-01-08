using System.Reflection;
using Backbone.Modules.Files.Infrastructure.Persistence;
using Backbone.Modules.Files.Jobs.SanityCheck.Extensions;
using Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.DataSource;
using Backbone.Modules.Files.Jobs.SanityCheck.Infrastructure.Reporter;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Files.Jobs.SanityCheck;

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
                    options.DbOptions.ConnectionString = configuration.GetSqlDatabaseConfiguration().ConnectionString;
                    options.DbOptions.Provider = configuration.GetSqlDatabaseConfiguration().Provider;

                    options.BlobStorageOptions.ConnectionInfo = configuration.GetBlobStorageConfiguration().ConnectionInfo;
                    options.BlobStorageOptions.CloudProvider = configuration.GetBlobStorageConfiguration().CloudProvider;
                    options.BlobStorageOptions.Container = configuration.GetBlobStorageConfiguration().ContainerName.IsNullOrEmpty() ? "files" : configuration.GetBlobStorageConfiguration().ContainerName;
                });
            });
    }
}
