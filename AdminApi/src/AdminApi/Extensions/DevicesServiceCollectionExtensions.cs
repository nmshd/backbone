using Backbone.AdminApi.Configuration;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;

namespace Backbone.AdminApi.Extensions;

public static class DevicesServiceCollectionExtensions
{
    public static IServiceCollection AddDevices(this IServiceCollection services,
        DevicesConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.Provider = configuration.Infrastructure.SqlDatabase.Provider;
            options.ConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services;
    }
}
