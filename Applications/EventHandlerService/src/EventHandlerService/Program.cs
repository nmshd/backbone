﻿using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Serilog;
using Backbone.EventHandlerService;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Files.Module;
using Backbone.Modules.Messages.Module;
using Backbone.Modules.Quotas.Module;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Module;
using Backbone.Modules.Tokens.Module;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using DevicesConfiguration = Backbone.Modules.Devices.Module.Configuration;


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
                .AddModule<DevicesModule>(configuration)
                .AddModule<RelationshipsModule>(configuration)
                .AddModule<ChallengesModule>(configuration)
                .AddModule<FilesModule>(configuration)
                .AddModule<MessagesModule>(configuration)
                .AddModule<QuotasModule>(configuration)
                .AddModule<SynchronizationModule>(configuration)
                .AddModule<TokensModule>(configuration);

            services.AddCustomIdentity(hostContext.HostingEnvironment);

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
            .Enrich.WithProperty("service", "eventHandlerService")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks())
        );
}
