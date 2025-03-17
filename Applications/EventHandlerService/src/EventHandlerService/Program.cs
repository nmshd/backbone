using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Serilog;
using Backbone.EventHandlerService;
using Backbone.Infrastructure.EventBus;
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
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using InfrastructureConfiguration = Backbone.Modules.Quotas.Module.InfrastructureConfiguration;


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
            services.ConfigureAndValidate<EventServiceConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
            var parsedConfiguration =
                services.BuildServiceProvider().GetRequiredService<IOptions<EventServiceConfiguration>>().Value;
#pragma warning restore ASP0000

            services.AddTransient<IHostedService, EventHandlerService>();

            services
                .AddModule<ChallengesModule, Backbone.Modules.Challenges.Application.ApplicationConfiguration, ChallengesInfrastructure>(configuration)
                .AddModule<DevicesModule, Backbone.Modules.Devices.Application.ApplicationConfiguration, Backbone.Modules.Devices.Module.InfrastructureConfiguration>(configuration)
                .AddModule<FilesModule, Backbone.Modules.Files.Application.ApplicationConfiguration, Backbone.Modules.Files.Module.InfrastructureConfiguration>(configuration)
                .AddModule<MessagesModule, Backbone.Modules.Messages.Application.ApplicationConfiguration, Backbone.Modules.Messages.Module.InfrastructureConfiguration>(configuration)
                .AddModule<QuotasModule, Backbone.Modules.Quotas.Application.ApplicationConfiguration, InfrastructureConfiguration>(configuration)
                .AddModule<RelationshipsModule, Backbone.Modules.Relationships.Application.ApplicationConfiguration,
                    Backbone.Modules.Relationships.Module.InfrastructureConfiguration>(configuration)
                .AddModule<SynchronizationModule, Backbone.Modules.Synchronization.Application.ApplicationConfiguration,
                    Backbone.Modules.Synchronization.Module.InfrastructureConfiguration>(configuration)
                .AddModule<TokensModule, ApplicationConfiguration, Backbone.Modules.Tokens.Module.InfrastructureConfiguration>(configuration);

            services.AddCustomIdentity(hostContext.HostingEnvironment);

            services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "eventHandlerService")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks())
        );
}
