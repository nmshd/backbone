using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.API;
using Backbone.API.Configuration;
using Backbone.API.Extensions;
using Backbone.API.Mvc.Middleware;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Tooling.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseKestrel(options =>
    {
        options.AddServerHeader = false;
        options.Limits.MaxRequestBodySize = 20.Mebibytes();
    });

LoadConfiguration(builder, args);

builder.Host
    .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();
Configure(app);

app
    .MigrateDbContext<Backbone.Modules.Challenges.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Backbone.Modules.Devices.Infrastructure.Persistence.Database.ApplicationDbContext>((context, _) => { new ApplicationDbContextSeed().SeedAsync(context).Wait(); })
    .MigrateDbContext<Backbone.Modules.Files.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Backbone.Modules.Relationships.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Backbone.Modules.Messages.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Backbone.Modules.Tokens.Infrastructure.Persistence.Database.ApplicationDbContext>();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services
        .ConfigureAndValidate<BackboneConfiguration>(configuration.Bind)
        .ConfigureAndValidate<Backbone.Modules.Challenges.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Challenges:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Devices.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Devices:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Files.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Files:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Messages.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Messages:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Relationships.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Relationships:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Synchronization.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Synchronization:Application").Bind(options))
        .ConfigureAndValidate<Backbone.Modules.Tokens.Application.ApplicationOptions>(options => configuration.GetSection("Modules:Tokens:Application").Bind(options));

#pragma warning disable ASP0000 We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<BackboneConfiguration>>().Value;
#pragma warning restore ASP0000

    services
        .AddCustomAspNetCore(parsedConfiguration, environment)
        .AddCustomApplicationInsights()
        .AddCustomIdentity(environment)
        .AddCustomFluentValidation()
        .AddCustomOpenIddict(parsedConfiguration.Authentication)
        .AddCustomSwaggerUi(parsedConfiguration.SwaggerUi);

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    // TODO: M switch to manual validation
    services.AddFluentValidationAutoValidation(config =>
    {
        config.DisableDataAnnotationsValidation = true;
        config.ImplicitlyValidateChildProperties = true;
        config.ImplicitlyValidateRootCollectionElements = true;
    });

    services
        .AddChallenges(parsedConfiguration.Modules.Challenges)
        .AddDevices(parsedConfiguration.Modules.Devices)
        .AddFiles(parsedConfiguration.Modules.Files)
        .AddMessages(parsedConfiguration.Modules.Messages)
        .AddRelationships(parsedConfiguration.Modules.Relationships)
        .AddSynchronization(parsedConfiguration.Modules.Synchronization)
        .AddTokens(parsedConfiguration.Modules.Tokens);

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
}

static void Configure(WebApplication app)
{
    var telemetryConfiguration = app.Services.GetRequiredService<TelemetryConfiguration>();
    telemetryConfiguration.DisableTelemetry = !app.Configuration.GetApplicationInsightsConfiguration().Enabled;

    app.UseForwardedHeaders();

    app.UseMiddleware<RequestResponseTimeMiddleware>()
        .UseMiddleware<ResponseDurationMiddleware>()
        .UseMiddleware<RequestIdMiddleware>();

    app.UseSecurityHeaders(policies =>
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny")
    );

    if (app.Environment.IsLocal() || app.Environment.IsDevelopment())
    {
        app.UseSwagger().UseSwaggerUI();
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.UseCors();

    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.AddSynchronizationIntegrationEventSubscriptions();
    eventBus.AddDevicesIntegrationEventSubscriptions();
}

static void LoadConfiguration(WebApplicationBuilder webApplicationBuilder, string[] strings)
{
    webApplicationBuilder.Configuration.Sources.Clear();
    var env = webApplicationBuilder.Environment;

    webApplicationBuilder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

    if (env.IsDevelopment())
    {
        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        webApplicationBuilder.Configuration.AddUserSecrets(appAssembly, optional: true);
    }

    webApplicationBuilder.Configuration.AddEnvironmentVariables();
    webApplicationBuilder.Configuration.AddCommandLine(strings);
    webApplicationBuilder.Configuration.AddAzureAppConfiguration();
}