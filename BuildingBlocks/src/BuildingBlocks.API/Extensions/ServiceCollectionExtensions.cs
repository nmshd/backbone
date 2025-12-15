using Asp.Versioning.ApiExplorer;
using Backbone.BuildingBlocks.API.AspNetCoreIdentityCustomizations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSqlDatabaseHealthCheck(string name, string provider, string connectionString)
        {
            switch (provider)
            {
                case DatabaseConfiguration.SQLSERVER:
                    services.AddHealthChecks().AddSqlServer(
                        connectionString,
                        name: $"{name}Database"
                    );
                    break;
                case DatabaseConfiguration.POSTGRES:
                    services.AddHealthChecks().AddNpgSql(
                        connectionString: connectionString,
                        name: $"{name}Database"
                    );
                    break;
                default:
                    throw new Exception($"Unsupported database provider: {provider}");
            }
        }

        public IServiceCollection AddModule<TModule, TApplicationConfiguration, TInfrastructureConfiguration>(IConfiguration configuration)
            where TModule : AbstractModule<TApplicationConfiguration, TInfrastructureConfiguration>, new()
            where TApplicationConfiguration : class
            where TInfrastructureConfiguration : class
        {
            var module = new TModule();

            var moduleConfiguration = configuration.GetSection($"Modules:{module.Name}");

            module.ConfigureServices(services, moduleConfiguration, configuration.GetSection("ModuleDefaults"));

            services.AddSingleton<IEventBusConfigurator>(module);

            return services;
        }

        public IServiceCollection AddCustomIdentity(IHostEnvironment environment)
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

                        options.Lockout.AllowedForNewUsers = true;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                        options.Lockout.MaxFailedAccessAttempts = 3;
                    }
                    else
                    {
                        options.Password.RequiredLength = 10;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireDigit = true;
                        options.Password.RequireNonAlphanumeric = true;

                        options.Lockout.AllowedForNewUsers = true;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                        options.Lockout.MaxFailedAccessAttempts = 3;
                    }
                })
                .AddEntityFrameworkStores<DevicesDbContext>()
                .AddSignInManager<CustomSigninManager>()
                .AddUserStore<CustomUserStore>();

            services.AddScoped<ILookupNormalizer, CustomLookupNormalizer>();

            return services;
        }

        public IServiceCollection AddOpenTelemetryWithPrometheusExporter(string name)
        {
            services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddPrometheusExporter()
                        .AddMeter(name)
                        .AddMeter("Microsoft.EntityFrameworkCore")
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Diagnostics")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                });

            return services;
        }

        public IServiceCollection AddCustomSwaggerUi(SwaggerUiConfiguration swaggerUi, string title)
        {
            if (!swaggerUi.Enabled)
                return services;

            services.AddEndpointsApiExplorer();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.Configure<SwaggerGenOptions>(options =>
            {
                foreach (var doc in options.SwaggerGeneratorOptions.SwaggerDocs.Values)
                {
                    doc.Title = title;
                }
            });

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(t =>
                {
                    static string GetReadableName(Type type)
                    {
                        if (!type.IsGenericType)
                        {
                            return type.Name
                                .Replace("DTO", string.Empty)
                                .Replace("Command", "Request")
                                .Replace("Query", "Request");
                        }

                        var typeName = type.Name
                            .Replace("HttpResponseEnvelopeResult", "ResponseWrapper")
                            .Replace("PagedHttpResponseEnvelopeResult", "PagedResponseWrapper");
                        var name = $"{typeName[..typeName.IndexOf('`')]}_{string.Join("_", type.GetGenericArguments().Select(GetReadableName))}";
                        return name;
                    }

                    return GetReadableName(t);
                });

                const string securityDefinitionName = "oauth2";

                options.AddSecurityDefinition(
                    securityDefinitionName,
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Password = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri("/connect/token", UriKind.Relative)
                            }
                        }
                    });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(securityDefinitionName, document)] = []
                });
            });

            services.AddApiVersioning().AddApiExplorer(options =>
            {
                // The specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}

public class SwaggerUiConfiguration
{
    public bool Enabled { get; init; }
}

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Version = description.ApiVersion.ToString(),
                });
        }
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse
                ? "default"
                : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
