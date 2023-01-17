using Autofac.Extensions.DependencyInjection;
using Autofac;
using Backbone.API.ApplicationInsights.TelemetryInitializers;
using Backbone.API.Certificates;
using Backbone.API.Mvc.ExceptionFilters;
using Backbone.API.Mvc.JsonConverters;
using Backbone.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.Tooling.JsonConverters;
using FluentValidation;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services,
        Configuration.BackboneConfiguration configuration,
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
                options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new UrlSafeBase64ToByteArrayJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DeviceIdJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new IdentityAddressJsonConverter());

                options.JsonSerializerOptions.Converters.Add(new ChallengeIdJsonConverter());

                // TODO: M add converters
            });

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidIssuer =
                    configuration.Authentication.ValidIssuer;

                // TODO: M Should we validate Audience?
                options.TokenValidationParameters.ValidateAudience = false;
                // options.TokenValidationParameters.ValidAudience = aspNetCoreOptions.Authentication.Audience;

                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.IssuerSigningKey =
                    JwtIssuerSigningKey.Get(configuration.Authentication, env);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true; // TODO: M rather false?
            });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration.Http.Cors.AllowedOrigins.Split(";"))
                    .WithExposedHeaders(configuration.Http.Cors.ExposedHeaders.Split(";"))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services
            .AddHealthChecks()
            .AddSqlServer(configuration.Services.Challenges.Infrastructure.SqlDatabase
                .ConnectionString); // TODO: use separate Connection String

        services.AddHttpContextAccessor();

        services.AddTransient<IUserContext, AspNetCoreUserContext>();

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
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member != null ? char.ToLowerInvariant(member.Name[0]) + member.Name[1..] : null;

        services.AddValidatorsFromAssemblies(new[] { typeof(Program).Assembly });

        return services;
    }

    public static AutofacServiceProvider ToAutofacServiceProvider(this IServiceCollection services)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }
}