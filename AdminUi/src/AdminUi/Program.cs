using System.Reflection;
using AdminUi.Authentication;
using AdminUi.Configuration;
using AdminUi.Extensions;
using AdminUi.Infrastructure.Persistence;
using AdminUi.Infrastructure.Persistence.Database;
using Autofac.Extensions.DependencyInjection;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;

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
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
        .Enrich.WithCorrelationId("X-Correlation-Id", addValueIfHeaderAbsence: true)
        .Enrich.WithDemystifiedStackTraces()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })))
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();
Configure(app);

app.MigrateDbContext<AdminUiDbContext>();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services.AddSaveChangesTimeInterceptor();

    services.AddSingleton<ApiKeyValidator>();

    services
        .ConfigureAndValidate<AdminConfiguration>(configuration.Bind)
        .ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Modules:Devices:Application").Bind(options));

#pragma warning disable ASP0000 We retrieve the Configuration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<AdminConfiguration>>().Value;

#pragma warning restore ASP0000

    services.AddCustomAspNetCore(parsedConfiguration)
        .AddCustomFluentValidation()
        .AddCustomIdentity(environment)
        .AddDatabase(parsedConfiguration.Infrastructure.SqlDatabase)
        .AddDevices(parsedConfiguration.Modules.Devices)
        .AddQuotas(parsedConfiguration.Modules.Quotas)
        .AddHealthChecks();

    if (parsedConfiguration.SwaggerUi.Enabled)
        services.AddCustomSwaggerWithUi();

    services
        .AddOpenIddict()
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<DevicesDbContext>()
                .ReplaceDefaultEntities<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreScope, CustomOpenIddictEntityFrameworkCoreToken, string>();
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

    if (env.IsDevelopment())
    {
        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        webApplicationBuilder.Configuration.AddUserSecrets(appAssembly, optional: true);
    }

    webApplicationBuilder.Configuration.AddEnvironmentVariables();
    webApplicationBuilder.Configuration.AddCommandLine(strings);
}

static void Configure(WebApplication app)
{
    app.UseForwardedHeaders();

    var configuration = app.Services.GetRequiredService<IOptions<AdminConfiguration>>().Value;

    app.UseSecurityHeaders(policies =>
    {
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny");

        if (configuration.Cors.AccessControlAllowCredentials)
            policies.AddCustomHeader("Access-Control-Allow-Credentials", "true");
    });

    if (configuration.SwaggerUi.Enabled)
    {
        app.UseSwagger().UseSwaggerUI();
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseCors();

    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapFallbackToFile("{*path:regex(^(?!api/).*$)}", "index.html"); // don't match paths beginning with "api/"

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });

    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.StartConsuming();
}

public partial class Program
{
}
