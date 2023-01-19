using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.API.ApplicationInsights.TelemetryInitializers;
using Devices.API.AspNetCoreIdentityCustomizations;
using Devices.API.Certificates;
using Devices.Application.Devices.DTOs;
using Devices.Domain.Entities;
using Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc.ExceptionFilters;
using Enmeshed.BuildingBlocks.API.Mvc.JsonConverters;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Infrastructure.UserContext;
using Enmeshed.Tooling.JsonConverters;
using FluentValidation;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devices.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddCustomAspNetCore(this IServiceCollection services, Action<AspNetCoreOptions> setupOptions)
    {
        var aspNetCoreOptions = new AspNetCoreOptions();
        setupOptions?.Invoke(aspNetCoreOptions);

        services
            .AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilter)))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstPropertyWithError = context.ModelState.First(p => p.Value != null && p.Value.Errors.Count > 0);
                    var nameOfPropertyWithError = firstPropertyWithError.Key;
                    var firstError = firstPropertyWithError.Value!.Errors.First();
                    var firstErrorMessage = !string.IsNullOrWhiteSpace(firstError.ErrorMessage)
                        ? firstError.ErrorMessage
                        : firstError.Exception != null
                            ? firstError.Exception.Message
                            : "";

                    var formattedMessage = string.IsNullOrEmpty(nameOfPropertyWithError) ? firstErrorMessage : $"'{nameOfPropertyWithError}': {firstErrorMessage}";
                    context.HttpContext.Response.ContentType = "application/json";
                    var responsePayload = new HttpResponseEnvelopeError(HttpError.ForProduction("error.platform.inputCannotBeParsed", formattedMessage, "")); // TODO: add docs
                    return new BadRequestObjectResult(responsePayload);
                };
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new UrlSafeBase64ToByteArrayJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DeviceIdJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new IdentityAddressJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new PublicKey.PublicKeyDTOJsonConverter()); 

                foreach (var converter in aspNetCoreOptions.Json.Converters)
                {
                    options.JsonSerializerOptions.Converters.Add(converter);
                }

                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

        services
            .AddAuthentication(IdentityServerConstants.LocalApi.AuthenticationScheme)
            .AddLocalApi(options => { options.ExpectedScope = "devices"; });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
            {
                policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(aspNetCoreOptions.Cors.AllowedOrigins.ToArray())
                    .WithExposedHeaders(aspNetCoreOptions.Cors.ExposedHeaders.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddHealthChecks().AddSqlServer(aspNetCoreOptions.HealthChecks.SqlConnectionString);

        services.AddHttpContextAccessor();

        services.AddTransient<IUserContext, AspNetCoreUserContext>();
    }

    public static void AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddIdentityServer(options =>
            {
                options.Endpoints.EnableAuthorizeEndpoint = false;
                options.Endpoints.EnableCheckSessionEndpoint = false;
                options.Endpoints.EnableDeviceAuthorizationEndpoint = false;
                options.Endpoints.EnableEndSessionEndpoint = false;
                options.Endpoints.EnableIntrospectionEndpoint = false;
                options.Endpoints.EnableJwtRequestUri = false;
                options.Endpoints.EnableTokenRevocationEndpoint = false;
                options.Endpoints.EnableUserInfoEndpoint = false;

                options.IssuerUri = configuration.GetAuthentication().IssuerUri;

                options.Caching.CorsExpiration = TimeSpan.FromDays(1);
                options.Caching.ResourceStoreExpiration = TimeSpan.FromDays(1);
            })
            .AddSigningCredential(Certificate.Get(configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(options =>
            {
                options.DefaultSchema = "Devices";
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(configuration.GetSqlDatabaseConfiguration().ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    });

                    if (environment.IsDevelopment())
                    {
                        builder.EnableDetailedErrors();
                        builder.EnableSensitiveDataLogging();
                    }
                };
            })
            .AddInMemoryCaching()
            .AddConfigurationStoreCache()
            .Services.AddTransient<IProfileService, ProfileService>();

        var allowedOrigins = configuration.GetCorsConfiguration().AllowedOrigins;

        if (allowedOrigins.Any())
            services.AddSingleton<ICorsPolicyService>(sp =>
                new DefaultCorsPolicyService(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DefaultCorsPolicyService>())
                {
                    AllowedOrigins = allowedOrigins
                });
    }

    public static void AddCustomIdentity(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                if (environment.IsDevelopment() || environment.IsLocal())
                {
                    options.Password.RequiredLength = 1;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                }
                else
                {
                    options.Password.RequiredLength = 10;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                }
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<CustomSigninManager>()
            .AddUserStore<CustomUserStore>();
    }

    public static void AddCustomApplicationInsights(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();

        services.AddSingleton(typeof(ITelemetryChannel), new ServerTelemetryChannel());
        TelemetryDebugWriter.IsTracingDisabled = true;

        services.AddSingleton<ITelemetryInitializer, UserInformationTelemetryInitializer>();
        services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();

        services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, _) =>
        {
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "alloc-rate"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "cpu-usage"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "exception-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-heap-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "working-set"));

            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Hosting", "current-requests"));

            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections", "connections-duration"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections", "current-connections"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections", "connections-timed-out"));

            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel", "connections-per-second"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel", "current-connections"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel", "request-queue-length"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel", "total-tls-handshakes"));
        });
    }

    public static void AddCustomFluentValidation(this IServiceCollection services, Action<FluentValidationOptions> setupOptions)
    {
        var fluentValidationOptions = new FluentValidationOptions();
        setupOptions?.Invoke(fluentValidationOptions);

        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member != null ? char.ToLowerInvariant(member.Name[0]) + member.Name[1..] : null;

        services.AddValidatorsFromAssemblies(fluentValidationOptions.ValidatorsAssemblies);
    }

    public class AspNetCoreOptions
    {
        public CorsOptions Cors { get; set; } = new();
        public JsonOptions Json { get; set; } = new();
        public AuthenticationOptions Authentication { get; set; } = new();
        public HealthCheckOptions HealthChecks { get; set; } = new();

        public class HealthCheckOptions
        {
            public string SqlConnectionString { get; set; }
        }

        public class CorsOptions
        {
            public List<string> AllowedOrigins { get; set; } = new();
            public List<string> ExposedHeaders { get; set; } = new();
        }

        public class JsonOptions
        {
            public List<JsonConverter> Converters { get; set; } = new();
        }

        public class AuthenticationOptions
        {
            public string Authority { get; set; }
            public string ValidIssuer { get; set; }
            public string Audience { get; set; }
        }
    }

    public class FluentValidationOptions
    {
        public ICollection<Assembly> ValidatorsAssemblies { get; set; } = new List<Assembly>();
    }
}
