using System.Reflection;
using Backbone.API;
using Backbone.API.Configuration;
using Backbone.API.Extensions;
using Backbone.API.Mvc.Middleware;
using Backbone.Infrastructure.EventBus;
using Challenges.Infrastructure.Persistence.Database;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.IdentityModel.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    .UseKestrel(options =>
    {
        options.AddServerHeader = false;
        options.Limits.MaxRequestBodySize = 0;
    });


builder.Configuration.Sources.Clear();
var env = builder.Environment;

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true); // TODO: make optional

if (env.IsDevelopment())
{
    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
    builder.Configuration.AddUserSecrets(appAssembly, optional: true);
}

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);
builder.Configuration.AddAzureAppConfiguration();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);
var app = builder.Build();
Configure(app);

app.MigrateDbContext<ApplicationDbContext>();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    var parsedConfiguration = new BackboneConfiguration();
    configuration.Bind(parsedConfiguration);

    services
        .AddCustomAspNetCore(parsedConfiguration, environment)
        .AddCustomApplicationInsights()
        .AddCustomFluentValidation();

    services.AddChallenges(parsedConfiguration.Services.Challenges);

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

    services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen();
}

static void Configure(WebApplication app)
{
    var telemetryConfiguration = app.Services.GetRequiredService<TelemetryConfiguration>();
    telemetryConfiguration.DisableTelemetry = !app.Configuration.GetApplicationInsightsConfiguration().Enabled;

    app.UseMiddleware<RequestResponseTimeMiddleware>()
        .UseMiddleware<ResponseDurationMiddleware>()
        .UseMiddleware<RequestIdMiddleware>();

    app.UseSecurityHeaders(policies =>
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny")
    );

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger().UseSwaggerUI();
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.UseCors();
}