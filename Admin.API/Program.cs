using Microsoft.Extensions.Options;
using System.Reflection;
using Admin.API.Configuration;
using Admin.API.Extensions;
using Enmeshed.Tooling.Extensions;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Logging;
using Backbone.Modules.Devices.Application;
using Backbone.Infrastructure.EventBus;

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

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services
        .ConfigureAndValidate<AdminConfiguration>(configuration.Bind)
        .ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Modules:Devices:Application").Bind(options));

#pragma warning disable ASP0000 We retrieve the Configuration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<AdminConfiguration>>().Value;
#pragma warning restore ASP0000

    services.AddCustomAspNetCore(parsedConfiguration, environment)
    .AddCustomFluentValidation()
    .AddCustomSwaggerWithUi()
    .AddDevices(parsedConfiguration.Modules.Devices)
    .AddQuotas(parsedConfiguration.Modules.Quotas)
    .AddHealthChecks();

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
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });
}