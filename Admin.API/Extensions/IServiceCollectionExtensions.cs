using Admin.API.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Admin.API.Extensions;

public static class IServiceCollectionExtensions
{
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