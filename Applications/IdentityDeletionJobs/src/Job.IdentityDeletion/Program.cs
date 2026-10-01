using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Files.Module;
using Backbone.Modules.Messages.Module;
using Backbone.Modules.Quotas.Module;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Module;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;

namespace Backbone.Job.IdentityDeletion;

public class Program
{
    private const string METER_NAME = "enmeshed.backbone.jobs.identitydeletion";

    public static async Task<int> Main(params string[] args)
    {
        using var startupLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            startupLogger.Information("Creating app...");

            var app = CreateHostBuilder(args).Build();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("App created.");
            logger.LogInformation("Starting app...");

            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            startupLogger.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.Sources.Clear();
                var env = hostContext.HostingEnvironment;

                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: false);

                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.ConfigureAndValidate<IdentityDeletionJobConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
                var parsedConfiguration =
                    services.BuildServiceProvider().GetRequiredService<IOptions<IdentityDeletionJobConfiguration>>().Value;
#pragma warning restore ASP0000

                var worker = Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t => t.Name == parsedConfiguration.Worker) ??
                             throw new ArgumentException("The specified worker could not be recognized, or no worker was set.");
                services.AddTransient(typeof(IHostedService), worker);

                services
                    .AddModule<AnnouncementsModule, Modules.Announcements.Module.ApplicationConfiguration, Modules.Announcements.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<ChallengesModule, Modules.Challenges.Module.ApplicationConfiguration, Modules.Challenges.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<DevicesModule, Modules.Devices.Application.ApplicationConfiguration, Modules.Devices.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<FilesModule, Modules.Files.Application.ApplicationConfiguration, Modules.Files.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<MessagesModule, Modules.Messages.Module.ApplicationConfiguration, Modules.Messages.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<QuotasModule, Modules.Quotas.Application.ApplicationConfiguration, Modules.Quotas.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<RelationshipsModule, Modules.Relationships.Application.ApplicationConfiguration,
                        Modules.Relationships.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<SynchronizationModule, Modules.Synchronization.Application.ApplicationConfiguration,
                        Modules.Synchronization.Infrastructure.InfrastructureConfiguration>(configuration)
                    .AddModule<TokensModule, Modules.Tokens.Application.ApplicationConfiguration, Modules.Tokens.Infrastructure.InfrastructureConfiguration>(configuration);

                services.AddSingleton<IDeletionProcessLogger, DeletionProcessLogger>();

                services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();

                services.AddCustomIdentity(hostContext.HostingEnvironment);

                services.RegisterIdentityDeleters();

                services.AddOpenTelemetry(configuration, METER_NAME);

                services.AddEventBus(parsedConfiguration.Infrastructure.EventBus, METER_NAME);
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Telemetry:Logging" })
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("service", "jobs.identitydeletion")
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers()
                        .WithDestructurers([new DbUpdateExceptionDestructurer()])
                    ), preserveStaticLogger: true
            );
    }
}
