using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.Backbone.Infrastructure.EventBus;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Common.Infrastructure;
using Backbone.ConsumerApi;
using Backbone.ConsumerApi.Configuration;
using Backbone.ConsumerApi.Extensions;
using Backbone.ConsumerApi.Mvc.Middleware;
using Backbone.Modules.Challenges.ConsumerApi;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.ConsumerApi;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.ConsumerApi;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.ConsumerApi;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.ConsumerApi;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.ConsumerApi;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.ConsumerApi;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.ConsumerApi;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseKestrel(options =>
    {
        options.AddServerHeader = false;
        options.Limits.MaxRequestBodySize = 20.Mebibytes();
    });

LoadConfiguration(builder, args);

builder.Host
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
        .Enrich.WithCorrelationId("X-Correlation-Id", addValueIfHeaderAbsence: true)
        .Enrich.WithDemystifiedStackTraces()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
    )
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
    .MigrateDbContext<QuotasDbContext>((context, sp) => { new QuotasDbContextSeed(sp.GetRequiredService<DevicesDbContext>()).SeedAsync(context).Wait(); })
    .MigrateDbContext<MessagesDbContext>()
    .MigrateDbContext<SynchronizationDbContext>()
    .MigrateDbContext<TokensDbContext>()
    .MigrateDbContext<QuotasDbContext>();

foreach (var module in app.Services.GetRequiredService<IEnumerable<AbstractModule>>())
{
    module.PostStartupValidation(app.Services);
}

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services.AddSaveChangesTimeInterceptor();

    services
        .AddModule<ChallengesModule>(configuration)
        .AddModule<DevicesModule>(configuration)
        .AddModule<FilesModule>(configuration)
        .AddModule<MessagesModule>(configuration)
        .AddModule<QuotasModule>(configuration)
        .AddModule<RelationshipsModule>(configuration)
        .AddModule<SynchronizationModule>(configuration)
        .AddModule<TokensModule>(configuration);

    var quotasSqlDatabaseConfiguration = configuration.GetSection("Modules:Quotas:Infrastructure:SqlDatabase");
    var quotasDbProvider = quotasSqlDatabaseConfiguration.GetValue<string>("Provider") ?? throw new ArgumentException("Quotas database connection string is not configured");
    var quotasDbConnectionString = quotasSqlDatabaseConfiguration.GetValue<string>("ConnectionString") ?? throw new ArgumentException("Quotas database connection string is not configured");
    services.AddMetricStatusesRepository(quotasDbProvider, quotasDbConnectionString);

    services.AddTransient<IQuotaChecker, QuotaCheckerImpl>();

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
        .AddCustomOpenIddict(parsedConfiguration.Authentication, environment);

    if (parsedConfiguration.SwaggerUi.Enabled)
        services.AddCustomSwaggerUi(parsedConfiguration.SwaggerUi);

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
        .UseMiddleware<TraceIdMiddleware>();

    app.UseSecurityHeaders(policies =>
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny")
    );

    var backboneConfiguration = app.Services.GetRequiredService<IOptions<BackboneConfiguration>>().Value;
    if (backboneConfiguration.SwaggerUi.Enabled)
    {
        app.UseSwagger().UseSwaggerUI();
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseCors();

    app.UseAuthentication().UseAuthorization();

    app.UseMiddleware<UserDataLoggingMiddleware>();

    app.MapControllers();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });

    var eventBus = app.Services.GetRequiredService<IEventBus>();
    var modules = app.Services.GetRequiredService<IEnumerable<AbstractModule>>();

    foreach (var module in modules)
    {
        module.ConfigureEventBus(eventBus);
    }

    eventBus.StartConsuming();
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

namespace Backbone.ConsumerApi
{
    public partial class Program
    {
    }
}
