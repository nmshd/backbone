using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomSwaggerUi(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "docs/{documentName}/openapi.json";
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_2;
        });

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "docs";

            // Build a endpoint for serving the openapi documents for each discovered API version
            // We reverse the order so that the highest version is shown first
            foreach (var description in app.DescribeApiVersions().Reverse())
            {
                var url = $"/docs/{description.GroupName}/openapi.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        return app;
    }
}
