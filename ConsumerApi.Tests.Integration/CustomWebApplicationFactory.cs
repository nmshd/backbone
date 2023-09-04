using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsumerApi.Tests.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            if (Environment.GetEnvironmentVariable("CI").IsNullOrEmpty())
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Modules:Challenges:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=challenges;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Quotas:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=quotas;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Devices:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=devices;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Files:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=files;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Messages:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=messages;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Relationships:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=relationships;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Synchronization:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=synchronization;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Tokens:Infrastructure:SqlDatabase:ConnectionString", "Server=localhost;Database=enmeshed;User Id=tokens;Password=Passw0rd;TrustServerCertificate=True"},
                    { "Modules:Tokens:Infrastructure:BlobStorage:ConnectionInfo", Environment.GetEnvironmentVariable("ENMESHED_BLOB_STORAGE_CONNECTION_STRING") },
                    { "Infrastructure:EventBus:ConnectionInfo", "localhost" }
                });
        });

        return base.CreateHost(builder);
    }
}
