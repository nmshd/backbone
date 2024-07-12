using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc.ExceptionFilters;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PublicKey = Backbone.Modules.Devices.Application.Devices.DTOs.PublicKey;

namespace Backbone.SseServer.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddCustomAspNetCore(this IServiceCollection services,
        Configuration configuration, IHostEnvironment env)
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilter)))
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

        services.AddAuthentication().AddJwtBearer(options =>
        {
            var privateKeyBytes = Convert.FromBase64String(configuration.Authentication.JwtSigningCertificate);
            var certificate = new X509Certificate2(privateKeyBytes, (string?)null);
            options.TokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
            options.TokenValidationParameters.ValidateIssuer = false;
            options.TokenValidationParameters.ValidateAudience = false;
        });
        services.AddAuthorization();

        services.AddHttpContextAccessor();

        services.AddTransient<IUserContext, AspNetCoreUserContext>();
    }
}
