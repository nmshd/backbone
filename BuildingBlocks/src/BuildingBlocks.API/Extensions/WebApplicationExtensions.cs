using Microsoft.AspNetCore.Builder;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomSwaggerUi(this WebApplication app)
    {
        app.UseSwagger(options => { options.RouteTemplate = "docs/{documentName}/openapi.json"; });

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "docs";

            // build a swagger endpoint for each discovered API version
            // we reverse the order so that the highest version is shown first
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
