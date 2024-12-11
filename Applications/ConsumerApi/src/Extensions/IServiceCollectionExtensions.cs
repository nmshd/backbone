using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;
using Backbone.BuildingBlocks.API.Mvc.ModelBinders;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.ConsumerApi.Configuration;
using Backbone.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenIddict.Validation.AspNetCore;
using PublicKey = Backbone.Modules.Devices.Application.Devices.DTOs.PublicKey;

namespace Backbone.ConsumerApi.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services, BackboneConfiguration configuration)
    {
        services
            .AddControllers(
                options =>
                {
                    options.Filters.Add(typeof(CustomExceptionFilter));

                    options.ModelBinderProviders.Insert(0, new GenericArrayModelBinderProvider());
                }
            )
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstPropertyWithError =
                        context.ModelState.First(p => p.Value is { Errors.Count: > 0 });
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
                        HttpError.ForProduction(GenericApplicationErrors.Validation.InputCannotBeParsed().Code, formattedMessage,
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

        services.AddAuthorizationBuilder()
            .AddPolicy(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
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

        if (configuration.Modules.Devices.Infrastructure.PushNotifications.Providers.Sse is { Enabled: true })
            services.AddHealthChecks().AddCheck<SseServerHealthCheck>("SseServer");

        return services;
    }

    public static IServiceCollection AddCustomOpenIddict(this IServiceCollection services,
        BackboneConfiguration.AuthenticationConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>()
                    .ReplaceDefaultEntities<
                        CustomOpenIddictEntityFrameworkCoreApplication,
                        CustomOpenIddictEntityFrameworkCoreAuthorization,
                        CustomOpenIddictEntityFrameworkCoreScope,
                        CustomOpenIddictEntityFrameworkCoreToken,
                        string
                    >();
            })
            .AddServer(options =>
            {
                var privateKeyBytes = Convert.FromBase64String(configuration.JwtSigningCertificate);
#pragma warning disable SYSLIB0057 // The constructor is obsolete. But I didn't manage to get the suggested alternative to work.
                var certificate = new X509Certificate2(privateKeyBytes, (string?)null);
#pragma warning restore SYSLIB0057
                options.AddSigningCertificate(certificate);

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
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member != null ? char.ToLowerInvariant(member.Name[0]) + member.Name[1..] : null;

        return services;
    }

    public static IServiceCollection AddCustomSwaggerUi(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }
}

public class SseServerHealthCheck : IHealthCheck
{
    private readonly HttpClient _client;

    public SseServerHealthCheck(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(nameof(SseServerClient));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetAsync("health", cancellationToken);
            return result.StatusCode == HttpStatusCode.OK ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}
