using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.Modules.Devices.Application.AutoMapper;
using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Jobs.IdentityDeletionGracePeriod.Extensions;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletionGracePeriod;
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

                var eventBusConfiguration = configuration.GetSection("Infrastructure").Get<InfrastructureConfiguration>().EventBus;
                var databaseConfiguration = configuration.GetSection("Infrastructure").Get<InfrastructureConfiguration>().SqlDatabase;

                services.AddEventBus(eventBusConfiguration);
                services.AddDatabase(options =>
                {
                    options.Provider = databaseConfiguration.Provider;
                    options.ConnectionString = databaseConfiguration.ConnectionString;
                    options.RetryOptions = databaseConfiguration.RetryOptions;
                });
                services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

                services.AddMediatR(o =>
                {
                    o.RegisterServicesFromAssemblyContaining<DeletionProcessGracePeriodCommand>();
                });

            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
