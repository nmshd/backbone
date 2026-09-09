using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Housekeeping;
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

using var host = CreateHostBuilder(args).Build();

try
{
    await host.StartAsync();

    await host.Services.GetRequiredService<Executor>().Execute(CancellationToken.None);

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "There was an error while executing the housekeeper.");
    return 1;
}
finally
{
    await host.StopAsync();
}

static HostApplicationBuilder CreateHostBuilder(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.ConfigureContainer(new AutofacServiceProviderFactory());

    var configuration = builder.Configuration;
    var services = builder.Services;

    // Configure Configuration
    builder.Configuration.Sources.Clear();
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.override.json", optional: true);
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddCommandLine(args);

    // Configure Services
    services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });

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

    services.AddCustomOpenIddict();

    services.AddCustomIdentity(builder.Environment);

    services.AddSingleton<Executor>();

    services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();

    services.AddOpenTelemetry(METER_NAME, Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown", parsedConfiguration.Telemetry.OpenTelemetryCollector,
        HousekeepingTelemetry.ACTIVITY_SOURCE_NAME);

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus, METER_NAME);

    services.AddSerilog(configuration => configuration
            .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions { SectionName = "Telemetry:Logging" })
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()])
            ), preserveStaticLogger: true
    );

    return builder;
}

public partial class Program
{
    private const string METER_NAME = "enmeshed.backbone.housekeeper";
}
