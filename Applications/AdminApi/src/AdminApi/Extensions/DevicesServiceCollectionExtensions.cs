using Backbone.AdminApi.Configuration;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Extensions;

public static class DevicesServiceCollectionExtensions
{
    public static IServiceCollection AddDevices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<DevicesConfiguration>>().Value;

        services.AddDatabase(options =>
        {
            options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services;
    }
}
