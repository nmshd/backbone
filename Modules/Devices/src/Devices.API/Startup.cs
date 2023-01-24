using Devices.API.Extensions;
using Devices.Application;
using Devices.Application.Extensions;
using Devices.Infrastructure.EventBus;
using Devices.Infrastructure.Persistence;
using Devices.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using IdentityServer4.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.HttpOverrides;

namespace Devices.API;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.Configure<ApplicationOptions>(_configuration.GetSection("ApplicationOptions"));

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddCustomAspNetCore(options =>
        {
            options.Authentication.Audience = "devices";
            options.Authentication.Authority = _configuration.GetAuthorizationConfiguration().Authority;
            options.Authentication.ValidIssuer = _configuration.GetAuthorizationConfiguration().ValidIssuer;

            options.Cors.AllowedOrigins = _configuration.GetCorsConfiguration().AllowedOrigins;
            options.Cors.ExposedHeaders = _configuration.GetCorsConfiguration().ExposedHeaders;

            options.HealthChecks.SqlConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString;
        });

        services.AddCustomApplicationInsights();

        services.AddCustomIdentity(_env);

        services.AddCustomIdentityServer(_configuration, _env);

        services.AddCustomFluentValidation(_ => { });

        services.AddDatabase(options => options.ConnectionString = _configuration.GetSqlDatabaseConfiguration().ConnectionString);

        services.AddEventBus(_configuration.GetEventBusConfiguration());

        services.AddPushNotifications(options =>
        {
            options.ConnectionString = _configuration.GetAzureNotificationHubsConfiguration().ConnectionString;
            options.HubName = _configuration.GetAzureNotificationHubsConfiguration().HubName;
        });

        services.AddApplication();

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services.ToAutofacServiceProvider();
    }

    public void Configure(IApplicationBuilder app, TelemetryConfiguration telemetryConfiguration)
    {
        telemetryConfiguration.DisableTelemetry = !_configuration.GetApplicationInsightsConfiguration().Enabled;

        app.Use(async (ctx, next) =>
        {
            ctx.SetIdentityServerOrigin(_configuration["AuthenticationConfiguration:PublicOrigin"]);
            await next();
        });

        app.UseForwardedHeaders();

        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.AddApplicationSubscriptions();

        app.ConfigureMiddleware(_env);

        app.UseIdentityServer();
    }
}
