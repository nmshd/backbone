﻿using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Backbone.AdminUi.Tests.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
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
