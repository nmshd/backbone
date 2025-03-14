using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Files.Module;
using Backbone.Modules.Messages.Module;
using Backbone.Modules.Quotas.Module;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Module;
using Backbone.Modules.Tokens.Module;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using Configuration = Backbone.Modules.Devices.Module.Configuration;

namespace Backbone.Job.IdentityDeletion;

public class Program
{
    public static async Task<int> Main(params string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Creating app...");

            var app = CreateHostBuilder(args);

            Log.Information("App created.");
            Log.Information("Starting app...");

            await app.Build().RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
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
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

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
                    .AddModule<AnnouncementsModule>(configuration)
                    .AddModule<DevicesModule>(configuration)
                    .AddModule<RelationshipsModule>(configuration)
                    .AddModule<ChallengesModule>(configuration)
                    .AddModule<FilesModule>(configuration)
                    .AddModule<MessagesModule>(configuration)
                    .AddModule<QuotasModule>(configuration)
                    .AddModule<SynchronizationModule>(configuration)
                    .AddModule<TokensModule>(configuration);

                services.AddSingleton<IDeletionProcessLogger, DeletionProcessLogger>();

                services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();
                services.AddFluentValidationAutoValidation(config => { config.DisableDataAnnotationsValidation = true; });

                services.AddCustomIdentity(hostContext.HostingEnvironment);

                services.RegisterIdentityDeleters();

                services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

                var devicesConfiguration = new Configuration();
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
                    .WithDestructurers([new DbUpdateExceptionDestructurer()])
                )
            );
    }
}
