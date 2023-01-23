using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.API.Configuration;
using Backbone.API.Extensions;
using Backbone.API.Mvc.Middleware;
using Backbone.Infrastructure.EventBus;
using Enmeshed.Tooling.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
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
    .MigrateDbContext<Challenges.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Files.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Messages.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Relationships.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Synchronization.Infrastructure.Persistence.Database.ApplicationDbContext>()
    .MigrateDbContext<Tokens.Infrastructure.Persistence.Database.ApplicationDbContext>();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services
        .Configure<Challenges.Application.ApplicationOptions>(configuration.GetSection("Modules:Challenges:Application"))
        .Configure<Files.Application.ApplicationOptions>(configuration.GetSection("Modules:Files:Application"))
        .Configure<Messages.Application.ApplicationOptions>(configuration.GetSection("Modules:Messages:Application"))
        .Configure<Relationships.Application.ApplicationOptions>(configuration.GetSection("Modules:Relationships:Application"))
        .Configure<Synchronization.Application.ApplicationOptions>(configuration.GetSection("Modules:Synchronization:Application"))
        .Configure<Tokens.Application.ApplicationOptions>(configuration.GetSection("Modules:Tokens:Application"));

    var parsedConfiguration = new BackboneConfiguration();
    configuration.Bind(parsedConfiguration);

    services
        .AddCustomAspNetCore(parsedConfiguration, environment)
        .AddCustomApplicationInsights()
        .AddCustomFluentValidation()
        .AddCustomSwaggerUI(parsedConfiguration.SwaggerUi);

    // TODO: M switch to manual validation
    services.AddFluentValidationAutoValidation(config =>
    {
        config.DisableDataAnnotationsValidation = true;
        config.ImplicitlyValidateChildProperties = true;
        config.ImplicitlyValidateRootCollectionElements = true;
    });

    services
        .AddChallenges(parsedConfiguration.Modules.Challenges)
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
}

static void LoadConfiguration(WebApplicationBuilder webApplicationBuilder, string[] strings)
{
    webApplicationBuilder.Configuration.Sources.Clear();
    var env = webApplicationBuilder.Environment;

    webApplicationBuilder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
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