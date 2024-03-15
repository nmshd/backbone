using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Challenges.Application.Identities;
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
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using DevicesConfiguration = Backbone.Modules.Devices.ConsumerApi.Configuration;

namespace Backbone.Job.IdentityDeletion;

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

                services.AddHostedService<ActualIdentityDeletionWorker>();
                services.AddHostedService<CancelIdentityDeletionProcessWorker>();

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

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
                var parsedConfiguration =
                    services.BuildServiceProvider().GetRequiredService<IOptions<IdentityDeletionJobConfiguration>>().Value;
#pragma warning restore ASP0000

                services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

                var devicesConfiguration = new DevicesConfiguration();
                configuration.GetSection("Modules:Devices").Bind(devicesConfiguration);
                services.AddPushNotifications(devicesConfiguration.Infrastructure.PushNotifications);
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "jobs.identitydeletion")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
        );
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Application.Identities.IdentityDeleter>();

        return services;
    }
}
