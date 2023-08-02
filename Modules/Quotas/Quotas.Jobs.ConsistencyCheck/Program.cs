using System.Reflection;
using Backbone.Modules.Quotas.Infrastructure.Persistence;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Extensions;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck;

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

                //services.AddScoped<IDataSource, DataSource>();

                //services.AddScoped<IReporter, LogReporter>();

                services.AddDatabase(options =>
                {
                    options.DbConnectionString = configuration.GetSqlDatabaseConfiguration().ConnectionString;
                    options.Provider = configuration.GetSqlDatabaseConfiguration().Provider;
                });
            });
    }
}
