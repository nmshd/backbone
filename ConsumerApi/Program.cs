using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using ConsumerApi;
using ConsumerApi.Configuration;
using ConsumerApi.Extensions;
using ConsumerApi.Mvc.Middleware;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Challenges.ConsumerApi;
using Devices.ConsumerApi;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Tooling.Extensions;
using Files.ConsumerApi;
using MediatR;
using Messages.ConsumerApi;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Quotas.ConsumerApi;
using Relationships.ConsumerApi;
using Serilog;
using Synchronization.ConsumerApi;
using Tokens.ConsumerApi;

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
    .MigrateDbContext<ChallengesDbContext>()
    .MigrateDbContext<DevicesDbContext>((context, sp) =>
    {
        var devicesDbContextSeed = new DevicesDbContextSeed(sp.GetRequiredService<IMediator>());
        devicesDbContextSeed.SeedAsync(context).Wait();
    })
    .MigrateDbContext<FilesDbContext>()
    .MigrateDbContext<RelationshipsDbContext>()
    .MigrateDbContext<QuotasDbContext>((context, sp) =>
    {
        new QuotasDbContextSeed(sp.GetRequiredService<DevicesDbContext>()).SeedAsync(context).Wait();
    })
    .MigrateDbContext<MessagesDbContext>()
    .MigrateDbContext<SynchronizationDbContext>()
    .MigrateDbContext<TokensDbContext>();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services
        .AddModule<ChallengesModule>(configuration)
        .AddModule<DevicesModule>(configuration)
        .AddModule<FilesModule>(configuration)
        .AddModule<MessagesModule>(configuration)
        .AddModule<QuotasModule>(configuration)
        .AddModule<RelationshipsModule>(configuration)
        .AddModule<SynchronizationModule>(configuration)
        .AddModule<TokensModule>(configuration);

    services.ConfigureAndValidate<BackboneConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedConfiguration =
        services.BuildServiceProvider().GetRequiredService<IOptions<BackboneConfiguration>>().Value;
#pragma warning restore ASP0000

    services
        .AddCustomAspNetCore(parsedConfiguration, environment)
        .AddCustomApplicationInsights()
        .AddCustomIdentity(environment)
        .AddCustomFluentValidation()
        .AddCustomOpenIddict(parsedConfiguration.Authentication, environment)
        .AddCustomSwaggerUi(parsedConfiguration.SwaggerUi);

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

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

    app.UseCors();

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });

    var eventBus = app.Services.GetRequiredService<IEventBus>();
    var modules = app.Services.GetRequiredService<IEnumerable<IModule>>();

    foreach (var module in modules)
    {
        module.ConfigureEventBus(eventBus);
    }
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