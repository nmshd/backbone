using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using FluentValidation.AspNetCore;

namespace Backbone.Job.IdentityDeletion
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
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
                    services.AddHostedService<CancelIdentityDeletionProcessWorker>();

                    services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();
                    services.AddFluentValidationAutoValidation(config => { config.DisableDataAnnotationsValidation = true; });

                    services.AddCustomIdentity(hostContext.HostingEnvironment);

                    services.ConfigureAndValidate<DeletionProcessJobConfiguration>(configuration.Bind);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
    }
}
