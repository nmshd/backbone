using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Backbone.AdminApi.Tests.Integration;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CustomWebApplicationFactory()
    {
        if (Environment.GetEnvironmentVariable("CI").IsNullOrEmpty())
            Environment.SetEnvironmentVariable(
                "BACKBONE_ADDITIONAL_CONFIGURATION_FILE",
                Path.Combine(AppContext.BaseDirectory, "api.appsettings.local.override.json"));
    }
}
