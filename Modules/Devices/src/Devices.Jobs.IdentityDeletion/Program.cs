using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.Modules.Devices.Jobs.IdentityDeletion.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().RunAsync();
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
                services.AddEventBus(configuration.GetSection("Infrastructure").Get<InfrastructureConfiguration>().EventBus);
            })
             .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
