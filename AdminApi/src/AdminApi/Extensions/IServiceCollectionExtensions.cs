using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.AdminApi.Authentication;
using Backbone.AdminApi.Configuration;
using Backbone.AdminApi.Filters;
using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Backbone.AdminApi.Extensions;

public static class IServiceCollectionExtensions
{
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

    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services,
        AdminConfiguration configuration)
    {
        services
            .AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(CustomExceptionFilter));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new RedirectAntiforgeryValidationFailedResultFilter());
            })
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

        services.AddAuthentication("ApiKey")
            .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationSchemeHandler>(
                "ApiKey",
                opts => opts.ApiKey = configuration.Authentication.ApiKey
            );

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiKey", policy =>
            {
                policy.AddAuthenticationSchemes("ApiKey");
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddAntiforgery(o =>
        {
            o.HeaderName = "X-XSRF-TOKEN";
            o.Cookie.Name = "X-XSRF-COOKIE";
            o.Cookie.HttpOnly = false;
        });

        var modules = configuration.Modules.GetType().GetProperties();
        foreach (var moduleProperty in modules)
        {
            var moduleName = moduleProperty.Name;
            var module = configuration.Modules.GetType().GetProperty(moduleName)!.GetValue(configuration.Modules, null)!;
            var provider = GetPropertyValue(module, "Infrastructure.SqlDatabase.Provider") as string;
            var connectionString = (string)GetPropertyValue(module, "Infrastructure.SqlDatabase.ConnectionString");

            switch (provider)
            {
                case "SqlServer":
                    services.AddHealthChecks().AddSqlServer(
                        connectionString,
                        name: moduleName
                        );
                    break;
                case "Postgres":
                    services.AddHealthChecks().AddNpgSql(
                        connectionString: connectionString,
                        name: moduleName);
                    break;
                default:
                    throw new Exception($"Unsupported database provider: {provider}");
            }
        }

        services.AddHttpContextAccessor();

        return services;
    }

    private static object GetPropertyValue(object source, string propertyPath)
    {
        foreach (var property in propertyPath.Split('.').Select(s => source.GetType().GetProperty(s)))
            source = property!.GetValue(source, null)!;

        return source;
    }

    public static IServiceCollection AddCustomSwaggerWithUi(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddOData(this IServiceCollection services)
    {
        var builder = new ODataConventionModelBuilder()
            .EnableLowerCamelCase();

        builder.EntitySet<IdentityOverview>("Identities")
            .EntityType.HasKey(identity => identity.Address);


        services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(100)
            .AddRouteComponents("odata", builder.GetEdmModel()));

        return services;
    }
}
