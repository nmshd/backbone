using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Challenges.ConsumerApi;
using Backbone.Modules.Devices.ConsumerApi;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Files.ConsumerApi;
using Backbone.Modules.Messages.ConsumerApi;
using Backbone.Modules.Quotas.ConsumerApi;
using Backbone.Modules.Relationships.ConsumerApi;
using Backbone.Modules.Synchronization.ConsumerApi;
using Backbone.Modules.Tokens.ConsumerApi;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using DevicesConfiguration = Backbone.Modules.Devices.ConsumerApi.Configuration;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;

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
                services.AddHostedService<Worker>();

                services
                    .AddModule<DevicesModule>(configuration)
                    .AddModule<RelationshipsModule>(configuration)
                    .AddModule<ChallengesModule>(configuration)
                    .AddModule<FilesModule>(configuration)
                    .AddModule<MessagesModule>(configuration)
                    .AddModule<QuotasModule>(configuration)
                    .AddModule<SynchronizationModule>(configuration)
                    .AddModule<TokensModule>(configuration);

                services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();
                services.AddFluentValidationAutoValidation(config => { config.DisableDataAnnotationsValidation = true; });

                services.AddCustomIdentity(hostContext.HostingEnvironment);

                services.RegisterIdentityDeleters();

                services.ConfigureAndValidate<IdentityDeletionJobConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 We retrieve the BackboneConfiguration via IOptions here so that it is validated
                var parsedConfiguration =
                    services.BuildServiceProvider().GetRequiredService<IOptions<IdentityDeletionJobConfiguration>>().Value;
#pragma warning restore ASP0000

                services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

                var devicesConfiguration = new DevicesConfiguration();
                configuration.GetSection("Modules:Devices").Bind(devicesConfiguration);
                services.AddPushNotifications(devicesConfiguration.Infrastructure.PushNotifications);
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, Challenges.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Messages.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Tokens.Application.Identities.IdentityDeleter>();

        return services;
    }
}
