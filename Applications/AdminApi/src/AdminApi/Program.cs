using Autofac.Extensions.DependencyInjection;
using Backbone.AdminApi.Authentication;
using Backbone.AdminApi.Configuration;
using Backbone.AdminApi.Extensions;
using Backbone.AdminApi.Infrastructure.Persistence;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.API.Serilog;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Module;
using Backbone.Modules.Challenges.Module;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Module;
using Backbone.Modules.Quotas.Module;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Module;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using InfrastructureConfiguration = Backbone.Modules.Devices.Infrastructure.InfrastructureConfiguration;
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
            .Enrich.WithProperty("service", "adminui")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks()), preserveStaticLogger: true)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    var app = builder.Build();
    Configure(app, app.Services.GetRequiredService<IOptions<AdminApiConfiguration>>().Value);

    if ((app.Environment.IsLocal() || app.Environment.IsDevelopment()) && app.Configuration.GetValue<bool>("RunMigrations"))
        app.MigrateDbContext<AdminApiDbContext>();

    return app;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services.AddSaveChangesTimeInterceptor();

    services.AddSingleton<ApiKeyValidator>();

    services.ConfigureAndValidate<AdminApiConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the Configuration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<AdminApiConfiguration>>().Value;
#pragma warning restore ASP0000

    services.AddCustomAspNetCore(parsedConfiguration)
        .AddOData()
        .AddCustomFluentValidation()
        .AddCustomIdentity(environment)
        .AddDatabase(parsedConfiguration.Infrastructure.SqlDatabase)
        .AddCustomSwaggerUi(parsedConfiguration.SwaggerUi, "Admin API")
        .AddHealthChecks();

    services
        .AddModule<AnnouncementsModule, Backbone.Modules.Announcements.Application.ApplicationConfiguration, Backbone.Modules.Announcements.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<ChallengesModule, Backbone.Modules.Challenges.Application.ApplicationConfiguration, Backbone.Modules.Challenges.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<DevicesModule, Backbone.Modules.Devices.Application.ApplicationConfiguration, InfrastructureConfiguration>(configuration)
        .AddModule<QuotasModule, Backbone.Modules.Quotas.Application.ApplicationConfiguration, Backbone.Modules.Quotas.Infrastructure.InfrastructureConfiguration>(configuration)
        .AddModule<TokensModule, ApplicationConfiguration, Backbone.Modules.Tokens.Infrastructure.InfrastructureConfiguration>(configuration);

    services
        .AddOpenIddict()
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<DevicesDbContext>()
                .ReplaceDefaultEntities<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreScope,
                    CustomOpenIddictEntityFrameworkCoreToken, string>();
            options.ReplaceApplicationStore<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreApplicationStore>();
        });

    services.AddOpenTelemetryWithPrometheusExporter(METER_NAME);

    services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus, METER_NAME);
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

static void Configure(WebApplication app, AdminApiConfiguration configuration)
{
    if (configuration.SwaggerUi.Enabled)
        app.UseCustomSwaggerUi();

    app.MapPrometheusScrapingEndpoint();

    // the following headers are necessary to run the application in webassembly mode
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "credentialless");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        await next.Invoke();
    });

    app.UseForwardedHeaders();

    app.UseMiddleware<RequestResponseTimeMiddleware>()
        .UseMiddleware<ResponseDurationMiddleware>()
        .UseMiddleware<TraceIdMiddleware>()
        .UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
        opts.GetLevel = LogHelper.GetLevel;
    });

    app.UseSecurityHeaders(policies =>
    {
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny");

        if (configuration.Cors is { AccessControlAllowCredentials: true })
            policies.AddCustomHeader("Access-Control-Allow-Credentials", "true");
    });

    if (app.Environment.IsDevelopment())
        IdentityModelEventSource.ShowPII = true;

    app.UseCors();

    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapFallbackToFile("{*path:regex(^(?!api/).*$)}", "index.html"); // don't match paths beginning with "api/"

    app.MapHealthChecks("/health");
}

public partial class Program
{
    private const string METER_NAME = "enmeshed.backbone.adminapi";
}
