using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac;
using Admin.API.Configuration;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Admin.API.Mvc.ExceptionFilters;
using Enmeshed.BuildingBlocks.API;
using Backbone.Infrastructure.UserContext;
using Admin.API.Certificates;
using Admin.API.AspNetCoreIdentityCustomizations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper.Internal;

namespace Admin.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAspNetCore(this IServiceCollection services, AdminConfiguration configuration)
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
                        .Where(
                            x => x.BaseType != null
                            && x.IsAssignableTo(typeof(JsonConverter))
                            // Ensure we only get classes that have a parameterless constructor because
                            // the creation of the instance will fail otherwise.
                            && x.GetConstructors().Any(y => y.GetParameters().Length == 0)
                        );

                foreach (var jsonConverter in jsonConverters)
                {
                    if (jsonConverter == typeof(DynamicJsonConverter)) continue;
                    var instance = (Activator.CreateInstance(jsonConverter) as JsonConverter)!;
                    options.JsonSerializerOptions.Converters.Add(instance);
                }

                options.JsonSerializerOptions.Converters.Add(new Backbone.Modules.Devices.Application.Devices.DTOs.PublicKey.PublicKeyDTOJsonConverter());
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

        services.AddTransient<IUserContext, AspNetCoreUserContext>();

        return services;
    }

    public static object GetPropertyValue(object source, string propertyPath)
    {
        foreach (var property in propertyPath.Split('.').Select(s => source.GetType().GetProperty(s)))
            source = property.GetValue(source, null);

        return source;
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
        AdminConfiguration.AuthenticationConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DevicesDbContext>();
            })
            .AddServer(options =>
            {
                options.AddSigningCertificate(Certificate.Get(configuration));
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

    public static IServiceCollection AddSwaggerWithCustomUi(this IServiceCollection services,
        AdminConfiguration.SwaggerUiConfiguration configuration)
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