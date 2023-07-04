using System.Reflection;
using AdminUi.Configuration;
using AdminUi.Extensions;
using Autofac.Extensions.DependencyInjection;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using static OpenIddict.Abstractions.OpenIddictExceptions;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.EntityFrameworkCore;

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

    services.AddCustomAspNetCore(parsedConfiguration)
        .AddCustomFluentValidation()
        .AddCustomIdentity(environment)
        .AddCustomSwaggerWithUi()
        .AddDevices(parsedConfiguration.Modules.Devices)
        .AddQuotas(parsedConfiguration.Modules.Quotas)
        .AddHealthChecks();

    services
        .AddOpenIddict()
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<DevicesDbContext>();
            options.AddApplicationStore<CustomOpenIddictEntityFrameworkCoreApplicationStore>();
        });
    services
        .AddTransient<IOpenIddictApplicationStore<OpenIddictEntityFrameworkCoreApplication>,
            CustomOpenIddictEntityFrameworkCoreApplicationStore>();

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
    app.MapFallbackToFile("{*path:regex(^(?!api/).*$)}", "index.html"); // don't match paths beginning with "api/"

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckWriter.WriteResponse
    });
}


/// <summary>
/// Provides methods allowing to manage the applications stored in a database.
/// </summary>
/// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
public class CustomOpenIddictEntityFrameworkCoreApplicationStore :
    OpenIddictEntityFrameworkCoreApplicationStore<DevicesDbContext>
{
    public CustomOpenIddictEntityFrameworkCoreApplicationStore(
        IMemoryCache cache,
        DevicesDbContext context,
        IOptionsMonitor<OpenIddictEntityFrameworkCoreOptions> options)
        : base(cache, context, options)
    {
    }

    public override async ValueTask DeleteAsync(OpenIddictEntityFrameworkCoreApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        Task<List<OpenIddictEntityFrameworkCoreAuthorization>> ListAuthorizationsAsync()
            => (from authorization in Context.Set<OpenIddictEntityFrameworkCoreAuthorization>().Include(authorization => authorization.Tokens)
                where authorization.Application!.Id!.Equals(application.Id)
                select authorization).ToListAsync(cancellationToken);

        Task<List<OpenIddictEntityFrameworkCoreToken>> ListTokensAsync()
            => (from token in Context.Set<OpenIddictEntityFrameworkCoreToken>()
                where token.Authorization == null
                where token.Application!.Id!.Equals(application.Id)
                select token).ToListAsync(cancellationToken);

        await Context.RunInTransaction(async () =>
        {
            // Remove all the authorizations associated with the application and
            // the tokens attached to these implicit or explicit authorizations.
            var authorizations = await ListAuthorizationsAsync();
            foreach (var authorization in authorizations)
            {
                foreach (var token in authorization.Tokens)
                {
                    Context.Set<OpenIddictEntityFrameworkCoreToken>().Remove(token);
                }

                Context.Set<OpenIddictEntityFrameworkCoreAuthorization>().Remove(authorization);
            }

            // Remove all the tokens associated with the application.
            var tokens = await ListTokensAsync();
            foreach (var token in tokens)
            {
                Context.Set<OpenIddictEntityFrameworkCoreToken>().Remove(token);
            }

            Context.Set<OpenIddictEntityFrameworkCoreApplication>().Remove(application);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }

            catch (DbUpdateConcurrencyException exception)
            {
                // Reset the state of the entity to prevents future calls to SaveChangesAsync() from failing.
                Context.Entry(application).State = EntityState.Unchanged;

                foreach (var authorization in authorizations)
                {
                    Context.Entry(authorization).State = EntityState.Unchanged;
                }

                foreach (var token in tokens)
                {
                    Context.Entry(token).State = EntityState.Unchanged;
                }

                throw
                    new ConcurrencyException("ERROR",
                        exception); //throw new ConcurrencyException(SR.GetResourceString(SR.ID0239), exception); //TODO: replace
            }
        }, new List<int>());
    }

}
