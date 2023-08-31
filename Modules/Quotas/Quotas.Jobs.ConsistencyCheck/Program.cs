using System.Reflection;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.DataSource;
using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Reporter;

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

                var quotasSqlDatabaseConfiguration = configuration.GetSection("SqlDatabase");
                var quotasDbProvider = quotasSqlDatabaseConfiguration.GetValue<string>("Provider") ?? throw new ArgumentException("Database provider is not configured");
                var quotasDbConnectionString = quotasSqlDatabaseConfiguration.GetValue<string>("ConnectionString") ?? throw new ArgumentException("Database connection string is not configured");
                services.AddConsistencyCheckRepository(quotasDbProvider, quotasDbConnectionString);
            });
    }
}
