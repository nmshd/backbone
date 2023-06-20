using System.Reflection;
using AdminUi.Configuration;
using AdminUi.Extensions;
using Autofac.Extensions.DependencyInjection;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
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
    // .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
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

    services.AddLogging();

    services.AddCustomAspNetCore(parsedConfiguration)
    .AddCustomFluentValidation()
    .AddCustomIdentity(environment)
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

    app.UseStaticFiles();
    app.UseRouting();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });
}
