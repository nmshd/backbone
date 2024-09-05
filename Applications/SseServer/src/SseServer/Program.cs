using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.ConsumerApi;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.SseServer.Controllers;
using Backbone.SseServer.Extensions;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using Configuration = Backbone.SseServer.Configuration;
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

    ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();
    Configure(app);

    foreach (var module in app.Services.GetRequiredService<IEnumerable<AbstractModule>>())
    {
        module.PostStartupValidation(app.Services);
    }

    return app;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.ConfigureAndValidate<Configuration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;
#pragma warning restore ASP0000

    services.AddSaveChangesTimeInterceptor();

    services.AddModule<DevicesModule>(configuration);

    services.AddSingleton<IEventQueue, EventQueue>();

    services.AddCustomAspNetCore(parsedConfiguration);

    services.AddScoped<IQuotaChecker, AlwaysSuccessQuotaChecker>();

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

    services.AddHealthChecks();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.AddPushNotifications(parsedConfiguration.Modules.Devices.Infrastructure.PushNotifications);
}

static void Configure(WebApplication app)
{
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

    if (app.Environment.IsDevelopment())
        IdentityModelEventSource.ShowPII = true;

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });

    app.UseResponseCaching();
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
}
