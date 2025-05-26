using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.AdminApi.Authentication;
using Backbone.AdminApi.Configuration;
using Backbone.AdminApi.DTOs;
using Backbone.AdminApi.Filters;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Backbone.AdminApi.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomFluentValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member != null ? char.ToLowerInvariant(member.Name[0]) + member.Name[1..] : null;

        return services;
    }

    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services,
        AdminApiConfiguration configuration)
    {
        services
            .AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(CustomExceptionFilter));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new RedirectAntiforgeryValidationFailedResultFilter());
            })
            .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory())
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

        services.AddAuthorizationBuilder()
            .AddPolicy("ApiKey", policy =>
            {
                policy.AddAuthenticationSchemes("ApiKey");
                policy.RequireAuthenticatedUser();
            });

        services.AddAntiforgery(o =>
        {
            o.HeaderName = "X-XSRF-TOKEN";
            o.Cookie.Name = "X-XSRF-COOKIE";
            o.Cookie.HttpOnly = false;
        });

        services.AddHttpContextAccessor();

        services.AddTransient<IUserContext, AnonymousUserContext>();

        return services;
    }

    private static Func<ActionContext, IActionResult> InvalidModelStateResponseFactory() => context =>
    {
        var (nameOfPropertyWithError, value) = context.ModelState.First(p => p.Value is { Errors.Count: > 0 });

        var firstError = value!.Errors.First();
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
            HttpError.ForProduction(GenericApplicationErrors.Validation.InputCannotBeParsed().Code,
                formattedMessage,
                "")); // TODO: add docs

        return new BadRequestObjectResult(responsePayload);
    };

    public static IServiceCollection AddOData(this IServiceCollection services)
    {
        var builder = new ODataConventionModelBuilder()
            .EnableLowerCamelCase();

        builder.EntitySet<IdentityOverviewDTO>("Identities").EntityType.HasKey(identity => identity.Address);

        services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(100)
            .AddRouteComponents("odata", builder.GetEdmModel()));

        return services;
    }
}
