using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.API.Serilog;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Common.Infrastructure;
using Backbone.ConsumerApi;
using Backbone.ConsumerApi.Configuration;
using Backbone.ConsumerApi.Extensions;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Module;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Module;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Module;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Module;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Module;
using Backbone.Modules.Tags.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Module;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using MyCSharp.HttpUserAgentParser.DependencyInjection;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using InfrastructureConfiguration = Backbone.Modules.Quotas.Infrastructure.InfrastructureConfiguration;
using LogHelper = Backbone.BuildingBlocks.API.Logging.LogHelper;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Creating app...");

    var app = CreateApp(args);

    Log.Information("App created.");

    Log.Information("Starting app...");

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static WebApplication CreateApp(string[] args)
{
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
            .Enrich.WithProperty("service", "consumerapi")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks()))
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    var app = builder.Build();
    Configure(app, app.Services.GetRequiredService<IOptions<ConsumerApiConfiguration>>().Value);

    if ((app.Environment.IsLocal() || app.Environment.IsDevelopment()) && app.Configuration.GetValue<bool>("RunMigrations"))
    {
        app
            .MigrateDbContext<AnnouncementsDbContext>()
            .MigrateDbContext<ChallengesDbContext>()
            .MigrateDbContext<DevicesDbContext>()
            .MigrateDbContext<FilesDbContext>()
            .MigrateDbContext<RelationshipsDbContext>()
            .MigrateDbContext<QuotasDbContext>()
            .MigrateDbContext<MessagesDbContext>()
            .MigrateDbContext<SynchronizationDbContext>()
            .MigrateDbContext<TokensDbContext>()
            .MigrateDbContext<QuotasDbContext>();
    }

    app
        .SeedDbContext<DevicesDbContext, DevicesDbContextSeeder>()
        .SeedDbContext<QuotasDbContext, QuotasDbContextSeeder>();

    return app;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services.ConfigureAndValidate<ConsumerApiConfiguration>(configuration.Bind);

    services.AddSingleton<VersionService>();

    services.AddSaveChangesTimeInterceptor();

    services.AddTransient<DevicesDbContextSeeder>();
    services.AddTransient<QuotasDbContextSeeder>();

    services
        .AddModule<AnnouncementsModule, Backbone.Modules.Announcements.Application.ApplicationConfiguration, Backbone.Modules.Announcements.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<ChallengesModule, Backbone.Modules.Challenges.Application.ApplicationConfiguration, Backbone.Modules.Challenges.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<DevicesModule, Backbone.Modules.Devices.Application.ApplicationConfiguration, Backbone.Modules.Devices.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<FilesModule, Backbone.Modules.Files.Application.ApplicationConfiguration, Backbone.Modules.Files.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<MessagesModule, Backbone.Modules.Messages.Application.ApplicationConfiguration, Backbone.Modules.Messages.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<QuotasModule, Backbone.Modules.Quotas.Application.ApplicationConfiguration, InfrastructureConfiguration>(configuration)
        .AddModule<RelationshipsModule, Backbone.Modules.Relationships.Application.ApplicationConfiguration,
            Backbone.Modules.Relationships.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<SynchronizationModule, Backbone.Modules.Synchronization.Application.ApplicationConfiguration,
            Backbone.Modules.Synchronization.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<TagsModule, Backbone.Modules.Tags.Application.ApplicationConfiguration, Backbone.Modules.Tags.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<TokensModule, ApplicationConfiguration, Backbone.Modules.Tokens.Infrastructure.InfrastructureConfiguration>(configuration);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedBackboneConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ConsumerApiConfiguration>>().Value;
    var parsedQuotasInfrastructureConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<InfrastructureConfiguration>>().Value;
#pragma warning restore ASP0000

    var quotasSqlDatabaseConfiguration = parsedQuotasInfrastructureConfiguration.SqlDatabase;
    services.AddMetricStatusesRepository(quotasSqlDatabaseConfiguration.Provider, quotasSqlDatabaseConfiguration.ConnectionString);

    services.AddTransient<IQuotaChecker, QuotaCheckerImpl>();

    services
        .AddCustomAspNetCore(parsedBackboneConfiguration)
        .AddCustomIdentity(environment)
        .AddCustomFluentValidation()
        .AddCustomOpenIddict(parsedBackboneConfiguration.Authentication)
        .AddCustomSwaggerUi(parsedBackboneConfiguration.SwaggerUi, "Consumer API");

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.AddOpenTelemetryWithPrometheusExporter(METER_NAME);

    services.AddEventBus(parsedBackboneConfiguration.Infrastructure.EventBus, METER_NAME);
    services.AddHttpUserAgentParser();
}

static void Configure(WebApplication app, ConsumerApiConfiguration configuration)
{
    if (configuration.SwaggerUi.Enabled)
        app.UseCustomSwaggerUi();

    app.MapPrometheusScrapingEndpoint();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
        opts.GetLevel = LogHelper.GetLevel;
    });

    app.UseStaticFiles();

    app.UseForwardedHeaders();

    app.UseMiddleware<RequestResponseTimeMiddleware>()
        .UseMiddleware<ResponseDurationMiddleware>()
        .UseMiddleware<TraceIdMiddleware>()
        .UseMiddleware<CorrelationIdMiddleware>();

    app.UseSecurityHeaders(policies =>
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny")
    );

    if (app.Environment.IsDevelopment())
        IdentityModelEventSource.ShowPII = true;

    app.UseCors();

    app.UseAuthentication().UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    app.UseResponseCaching();
}

static void LoadConfiguration(WebApplicationBuilder webApplicationBuilder, string[] strings)
{
    webApplicationBuilder.Configuration.Sources.Clear();
    var env = webApplicationBuilder.Environment;

    webApplicationBuilder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: false);

    webApplicationBuilder.Configuration.AddEnvironmentVariables();
    webApplicationBuilder.Configuration.AddCommandLine(strings);
}

public partial class Program
{
    private const string METER_NAME = "enmeshed.backbone.consumerapi";
}
