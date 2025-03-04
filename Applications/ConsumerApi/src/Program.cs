using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.API.Serilog;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Common.Infrastructure;
using Backbone.ConsumerApi;
using Backbone.ConsumerApi.Configuration;
using Backbone.ConsumerApi.Extensions;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Announcements.ConsumerApi;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.ConsumerApi;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.ConsumerApi;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Files.ConsumerApi;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.ConsumerApi;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.ConsumerApi;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.ConsumerApi;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.ConsumerApi;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tags.ConsumerApi;
using Backbone.Modules.Tokens.ConsumerApi;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
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
            .Enrich.WithProperty("service", "consumerapi")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithSensitiveDataMasking(options => options.AddSensitiveDataMasks()))
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    var app = builder.Build();
    Configure(app);

    if ((app.Environment.IsLocal() || app.Environment.IsDevelopment()) && app.Configuration.GetValue<bool>("RunMigrations"))
    {
        app
            .MigrateDbContext<AnnouncementsDbContext>()
            .MigrateDbContext<ChallengesDbContext>()
            .MigrateDbContext<DevicesDbContext>()
            .MigrateDbContext<FilesDbContext>()
            .MigrateDbContext<RelationshipsDbContext>()
            .MigrateDbContext<QuotasDbContext>()
            .MigrateDbContext<MessagesDbContext>()
            .MigrateDbContext<SynchronizationDbContext>()
            .MigrateDbContext<TokensDbContext>()
            .MigrateDbContext<QuotasDbContext>();
    }

    app
        .SeedDbContext<DevicesDbContext, DevicesDbContextSeeder>()
        .SeedDbContext<QuotasDbContext, QuotasDbContextSeeder>();

    foreach (var module in app.Services.GetRequiredService<IEnumerable<AbstractModule>>())
    {
        module.PostStartupValidation(app.Services);
    }

    return app;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    services.ConfigureAndValidate<BackboneConfiguration>(configuration.Bind);

#pragma warning disable ASP0000 // We retrieve the BackboneConfiguration via IOptions here so that it is validated
    var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<BackboneConfiguration>>().Value;
#pragma warning restore ASP0000

    services.AddSingleton<VersionService>();

    services.AddSaveChangesTimeInterceptor();

    services.AddTransient<DevicesDbContextSeeder>();
    services.AddTransient<QuotasDbContextSeeder>();

    services
        .AddModule<AnnouncementsModule>(configuration)
        .AddModule<ChallengesModule>(configuration)
        .AddModule<DevicesModule>(configuration)
        .AddModule<FilesModule>(configuration)
        .AddModule<MessagesModule>(configuration)
        .AddModule<QuotasModule>(configuration)
        .AddModule<RelationshipsModule>(configuration)
        .AddModule<SynchronizationModule>(configuration)
        .AddModule<TagsModule>(configuration)
        .AddModule<TokensModule>(configuration);

    var quotasSqlDatabaseConfiguration = parsedConfiguration.Modules.Quotas.Infrastructure.SqlDatabase;
    services.AddMetricStatusesRepository(quotasSqlDatabaseConfiguration.Provider, quotasSqlDatabaseConfiguration.ConnectionString);

    services.AddTransient<IQuotaChecker, QuotaCheckerImpl>();

    services
        .AddCustomAspNetCore(parsedConfiguration)
        .AddCustomIdentity(environment)
        .AddCustomFluentValidation()
        .AddCustomOpenIddict(parsedConfiguration.Authentication);

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);

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
        .UseMiddleware<TraceIdMiddleware>()
        .UseMiddleware<CorrelationIdMiddleware>();

    app.UseSecurityHeaders(policies =>
        policies
            .AddDefaultSecurityHeaders()
            .AddCustomHeader("Strict-Transport-Security", "max-age=5184000; includeSubDomains")
            .AddCustomHeader("X-Frame-Options", "Deny")
    );

    if (app.Environment.IsDevelopment())
        IdentityModelEventSource.ShowPII = true;

    app.UseCors();

    app.UseAuthentication().UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

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

    webApplicationBuilder.Configuration.AddEnvironmentVariables();
    webApplicationBuilder.Configuration.AddCommandLine(strings);
}
