using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.API.ApplicationInsights.TelemetryInitializers;
using Backbone.API.Certificates;
using Backbone.API.Configuration;
using Backbone.API.Mvc.ExceptionFilters;
using Backbone.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using FluentValidation;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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
                    var instance = (Activator.CreateInstance(jsonConverter) as JsonConverter)!;
                    options.JsonSerializerOptions.Converters.Add(instance);
                }
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
                options.SaveToken = false;
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

        services
            .AddHealthChecks()
            .AddSqlServer(configuration.Modules.Challenges.Infrastructure.SqlDatabase
                .ConnectionString); // TODO: M use separate Connection String

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

        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    public static IServiceCollection AddCustomSwaggerUI(this IServiceCollection services, BackboneConfiguration.SwaggerUiConfiguration configuration)
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
                    {securityScheme, new string[] { }}
                });
            });

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