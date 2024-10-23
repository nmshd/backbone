using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Backbone.AdminApi.Tests.Integration;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            if (Environment.GetEnvironmentVariable("CI").IsNullOrEmpty())
                config.AddJsonFile("api.appsettings.local.override.json");
        });

        return base.CreateHost(builder);
    }
}
