using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.Housekeeper;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Files.Module;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Infrastructure;
using Backbone.Modules.Tokens.Module;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var app = CreateHostBuilder(args).Build();

await app.Services.GetRequiredService<Executor>().Execute(CancellationToken.None);
return;

static IHostBuilder CreateHostBuilder(string[] args)
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

            services.ConfigureAndValidate<Configuration>(configuration.Bind);

            services.AddLogging();

            services
                .AddModule<AnnouncementsModule, Backbone.Modules.Announcements.Application.ApplicationConfiguration,
                    Backbone.Modules.Announcements.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<ChallengesModule, Backbone.Modules.Challenges.Application.ApplicationConfiguration, Backbone.Modules.Challenges.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<DevicesModule, Backbone.Modules.Devices.Application.ApplicationConfiguration, Backbone.Modules.Devices.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<FilesModule, Backbone.Modules.Files.Application.ApplicationConfiguration, Backbone.Modules.Files.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<RelationshipsModule, Backbone.Modules.Relationships.Application.ApplicationConfiguration,
                    Backbone.Modules.Relationships.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<SynchronizationModule, Backbone.Modules.Synchronization.Application.ApplicationConfiguration,
                    Backbone.Modules.Synchronization.Infrastructure.InfrastructureConfiguration>(configuration)
                .AddModule<TokensModule, ApplicationConfiguration, InfrastructureConfiguration>(configuration);

            var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

            services.AddSingleton<Executor>();

            services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();

            services.AddEventBus(parsedConfiguration.Infrastructure.EventBus, METER_NAME);
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "housekeeper")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()])
            )
        );
}

public partial class Program
{
    private const string METER_NAME = "enmeshed.backbone.housekeeper";
}
