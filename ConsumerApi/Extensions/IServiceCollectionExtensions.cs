using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.ConsumerApi.Configuration;
using Backbone.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using PublicKey = Backbone.Modules.Devices.Application.Devices.DTOs.PublicKey;

namespace Backbone.ConsumerApi.Extensions;

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
                        .Where(x => !x.ContainsGenericParameters)
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

    public static IServiceCollection AddCustomOpenIddict(this IServiceCollection services,
        BackboneConfiguration.AuthenticationConfiguration configuration, IHostEnvironment environment)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>()
                    .ReplaceDefaultEntities<CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreScope, CustomOpenIddictEntityFrameworkCoreToken, string>();
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
}
