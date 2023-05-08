using System.Text.Json;
using System.Text.Json.Serialization;
using Admin.API.Configuration;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Enmeshed.BuildingBlocks.API.Mvc.ExceptionFilters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Admin.API.Extensions;

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
        AdminConfiguration configuration,
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

        var modules = configuration.Modules.GetType().GetProperties();
        foreach (var moduleProperty in modules)
        {
            if (moduleProperty is null) continue;

            var moduleName = moduleProperty.Name;
            var module = configuration.Modules.GetType().GetProperty(moduleName).GetValue(configuration.Modules, null);
            var provider = GetPropertyValue(module, "Infrastructure.SqlDatabase.Provider") as string;
            var connectionString = GetPropertyValue(module, "Infrastructure.SqlDatabase.ConnectionString") as string;

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
                        npgsqlConnectionString: connectionString,
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
            source = property.GetValue(source, null);

        return source;
    }

    public static IServiceCollection AddCustomSwaggerWithUi(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }
}