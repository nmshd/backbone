using Backbone.Crypto.Abstractions;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Google;
using Microsoft.EntityFrameworkCore;

namespace Backbone.ConsumerApi.Tests.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            if (Environment.GetEnvironmentVariable("CI").IsNullOrEmpty())
                config.AddJsonFile("api.appsettings.local.override.json");
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType ==  typeof(ISignatureHelper));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<ISignatureHelper, DummySignatureHelper>();
        });

        return base.CreateHost(builder);
    }
}

public class DummySignatureHelper : ISignatureHelper
{
    public KeyPair CreateKeyPair()
    {
        throw new NotImplementedException();
    }

    public bool VerifySignature(ConvertibleString message, ConvertibleString signature, ConvertibleString publicKey)
    {
        return true;
    }

    public ConvertibleString GetSignature(ConvertibleString privateKey, ConvertibleString message)
    {
        throw new NotImplementedException();
    }

    public bool IsValidPublicKey(ConvertibleString publicKey)
    {
        return true;
    }

    public bool IsValidPrivateKey(ConvertibleString privateKey)
    {
        return true;
    }
}
