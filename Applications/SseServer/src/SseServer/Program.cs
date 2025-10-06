using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Infrastructure;
using Backbone.Modules.Devices.Module;
using Backbone.SseServer.Controllers;
using Backbone.SseServer.Extensions;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using Configuration = Backbone.SseServer.Configuration;
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
            options.Limits.MaxRequestBodySize = 2.Kibibytes();
        });

    LoadConfiguration(builder, args);

    builder.Host
        .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
            .Enrich.WithCorrelationId("X-Correlation-Id", addValueIfHeaderAbsence: true)
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "sseserver")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
        )
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    var app = builder.Build();
    Configure(app, app.Services.GetRequiredService<IOptions<Configuration>>().Value);

    return app;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    services.ConfigureAndValidate<Configuration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;
#pragma warning restore ASP0000

    services.AddSaveChangesTimeInterceptor();

    services.AddModule<DevicesModule, ApplicationConfiguration, InfrastructureConfiguration>(configuration);

    services.AddSingleton<IEventQueue, EventQueue>();

    services.AddCustomAspNetCore(parsedConfiguration);
    services.AddCustomSwaggerUi(parsedConfiguration.SwaggerUi, "SSE Server");

    services.AddScoped<IQuotaChecker, AlwaysSuccessQuotaChecker>();

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus, METER_NAME);

    services.AddHealthChecks();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.AddOpenTelemetryWithPrometheusExporter(METER_NAME);

    services.AddCustomIdentity(environment);

    services.Configure(parsedConfiguration.SseServer);
}

static void Configure(WebApplication app, Configuration configuration)
{
    if (configuration.SwaggerUi.Enabled)
        app.UseCustomSwaggerUi();

    app.MapPrometheusScrapingEndpoint();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
        opts.GetLevel = LogHelper.GetLevel;
    });

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

    app.UseCors();

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health");
}

static void LoadConfiguration(WebApplicationBuilder webApplicationBuilder, string[] strings)
{
    webApplicationBuilder.Configuration.Sources.Clear();
    var env = webApplicationBuilder.Environment;

    webApplicationBuilder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

    webApplicationBuilder.Configuration.AddEnvironmentVariables();
    webApplicationBuilder.Configuration.AddCommandLine(strings);
}

public partial class Program
{
    private const string METER_NAME = "enmeshed.backbone.sseserver";
}
