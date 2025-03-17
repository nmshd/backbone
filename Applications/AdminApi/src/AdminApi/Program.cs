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
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Infrastructure.EventBus;
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
using InfrastructureConfiguration = Backbone.Modules.Devices.Module.InfrastructureConfiguration;
using LogHelper = Backbone.Infrastructure.Logging.LogHelper;

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
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks()))
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    var app = builder.Build();
    Configure(app);

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
        .AddHealthChecks();


    services
        .AddModule<AnnouncementsModule, Backbone.Modules.Announcements.Application.ApplicationConfiguration, Backbone.Modules.Announcements.Module.InfrastructureConfiguration>(configuration)
        .AddModule<ChallengesModule, Backbone.Modules.Challenges.Application.ApplicationConfiguration, ChallengesInfrastructure>(configuration)
        .AddModule<DevicesModule, Backbone.Modules.Devices.Application.ApplicationConfiguration, InfrastructureConfiguration>(configuration)
        .AddModule<QuotasModule, Backbone.Modules.Quotas.Application.ApplicationConfiguration, Backbone.Modules.Quotas.Module.InfrastructureConfiguration>(configuration)
        .AddModule<TokensModule, ApplicationConfiguration, Backbone.Modules.Tokens.Module.InfrastructureConfiguration>(configuration);


    services
        .AddOpenIddict()
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<DevicesDbContext>()
                .ReplaceDefaultEntities<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreScope,
                    CustomOpenIddictEntityFrameworkCoreToken, string>();
            options.AddApplicationStore<CustomOpenIddictEntityFrameworkCoreApplicationStore>();
        });

    services.AddTransient<IQuotaChecker, AlwaysSuccessQuotaChecker>();

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
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

static void Configure(WebApplication app)
{
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

    var configuration = app.Services.GetRequiredService<IOptions<AdminApiConfiguration>>().Value;

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

        if (configuration.Cors.AccessControlAllowCredentials)
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
