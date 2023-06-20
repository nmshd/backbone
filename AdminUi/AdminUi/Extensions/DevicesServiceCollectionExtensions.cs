using AdminApi.Configuration;
using Backbone.Modules.Devices.Application.Extensions;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using Backbone.Modules.Devices.Infrastructure.Persistence;

namespace AdminApi.Extensions;

public static class DevicesServiceCollectionExtensions
{
    public static IServiceCollection AddDevices(this IServiceCollection services,
        DevicesConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.ConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
            options.Provider = configuration.Infrastructure.SqlDatabase.Provider;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services;
    }
}