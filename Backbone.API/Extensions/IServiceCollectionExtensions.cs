﻿using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.API.ApplicationInsights.TelemetryInitializers;
using Backbone.API.AspNetCoreIdentityCustomizations;
using Backbone.API.Configuration;
using Backbone.API.Mvc.ExceptionFilters;
using Backbone.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.Tooling.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using IStartup = Enmeshed.BuildingBlocks.API.IStartup;
using PublicKey = Backbone.Modules.Devices.Application.Devices.DTOs.PublicKey;

namespace Backbone.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services,
        BackboneConfiguration configuration,
        IHostEnvironment env)
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilter)))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstPropertyWithError =
                        context.ModelState.First(p => p.Value != null && p.Value.Errors.Count > 0);
                    var nameOfPropertyWithError = firstPropertyWithError.Key;
                    var firstError = firstPropertyWithError.Value!.Errors.First();
                    var firstErrorMessage = !string.IsNullOrWhiteSpace(firstError.ErrorMessage)
                        ? firstError.ErrorMessage
                        : firstError.Exception != null
                            ? firstError.Exception.Message
                            : "";

                    var formattedMessage = string.IsNullOrEmpty(nameOfPropertyWithError)
                        ? firstErrorMessage
                        : $"'{nameOfPropertyWithError}': {firstErrorMessage}";
                    context.HttpContext.Response.ContentType = "application/json";
                    var responsePayload = new HttpResponseEnvelopeError(
                        HttpError.ForProduction("error.platform.inputCannotBeParsed", formattedMessage,
                            "")); // TODO: add docs
                    return new BadRequestObjectResult(responsePayload);
                };
            })
            .AddJsonOptions(options =>
            {
                var jsonConverters =
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(x => x.ExportedTypes)
                        .Where(x => !x.IsAbstract)
                        .Where(x => x.BaseType != null && x.IsAssignableTo(typeof(JsonConverter)));

                foreach (var jsonConverter in jsonConverters)
                {
                    if (jsonConverter == typeof(DynamicJsonConverter)) continue;
                    var instance = (Activator.CreateInstance(jsonConverter) as JsonConverter)!;
                    options.JsonSerializerOptions.Converters.Add(instance);
                }

                options.JsonSerializerOptions.Converters.Add(new PublicKey.PublicKeyDTOJsonConverter());
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration.Cors.AllowedOrigins.Split(";"))
                    .WithExposedHeaders(configuration.Cors.ExposedHeaders.Split(";"))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddHttpContextAccessor();

        services.AddTransient<IUserContext, AspNetCoreUserContext>();

        return services;
    }
    
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IHostEnvironment environment)
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

                    options.User.AllowedUserNameCharacters += " ";
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
            .AddEntityFrameworkStores<DevicesDbContext>()
            .AddSignInManager<CustomSigninManager>()
            .AddUserStore<CustomUserStore>();

        services.AddScoped<ILookupNormalizer, CustomLookupNormalizer>();

        return services;
    }

    public static IServiceCollection AddCustomOpenIddict(this IServiceCollection services,
        BackboneConfiguration.AuthenticationConfiguration configuration, IHostEnvironment environment)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>();
            })
            .AddServer(options =>
            {
                if (configuration.JwtSigningCertificate.IsNullOrEmpty())
                {
                    if (environment.IsProduction())
                        throw new Exception(
                            $"For production scenarios, you need to set a '{nameof(configuration.JwtSigningCertificate)}'.");

                    Log.Logger.Warning("Using development signing certificate. Note that this is not recommended for production scenarios!");
                    options.AddDevelopmentSigningCertificate();
                }
                else
                {
                    var privateKeyBytes = Convert.FromBase64String(configuration.JwtSigningCertificate);
                    var certificate = new X509Certificate2(privateKeyBytes, (string?)null);
                    options.AddSigningCertificate(certificate);
                }
                options.SetTokenEndpointUris("connect/token");
                options.AllowPasswordFlow();
                options.SetAccessTokenLifetime(TimeSpan.FromSeconds(configuration.JwtLifetimeInSeconds));

                options.DisableAccessTokenEncryption();

                options.AddDevelopmentEncryptionCertificate(); // for some reason this is necessary even though we don't encrypt the tokens

                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .DisableTransportSecurityRequirement();

                options.DisableTokenStorage();
            })
            .AddValidation(options =>
            {
                // import the configuration (like valid issuer and the signing certificate) from the local OpenIddict server instance.
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }

    public static IServiceCollection AddCustomApplicationInsights(this IServiceCollection services)
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

            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections",
                "connections-duration"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections",
                "current-connections"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft.AspNetCore.Http.Connections",
                "connections-timed-out"));

            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel",
                "connections-per-second"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel",
                "current-connections"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel",
                "request-queue-length"));
            module.Counters.Add(new EventCounterCollectionRequest("Microsoft-AspNetCore-Server-Kestrel",
                "total-tls-handshakes"));
        });

        return services;
    }

    public static IServiceCollection AddCustomFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(config =>
        {
            config.DisableDataAnnotationsValidation = true;
        });

        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member != null ? char.ToLowerInvariant(member.Name[0]) + member.Name[1..] : null;

        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    public static IServiceCollection AddCustomSwaggerUi(this IServiceCollection services,
        BackboneConfiguration.SwaggerUiConfiguration configuration)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows { Password = new OpenApiOAuthFlow { TokenUrl = new Uri(configuration.TokenUrl) } },
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    UnresolvedReference = false,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

        return services;
    }

    public static IServiceCollection AddModule<TStartup>(this IServiceCollection services, string moduleName, IConfiguration configuration)
        where TStartup : Enmeshed.BuildingBlocks.API.IStartup, new()
    {
        // Register assembly in MVC so it can find controllers of the module
        services.AddControllers().ConfigureApplicationPartManager(manager =>
            manager.ApplicationParts.Add(new AssemblyPart(typeof(TStartup).Assembly)));

        var startup = new TStartup();

        var moduleConfiguration = configuration.GetSection($"Modules:{moduleName}");

        startup.ConfigureServices(services, moduleConfiguration);

        services.AddSingleton(new Module { Name = moduleName, Startup = startup });

        return services;
    }
}

public class Module
{
    public string Name { get; init; }
    public IStartup Startup { get; init; }
}