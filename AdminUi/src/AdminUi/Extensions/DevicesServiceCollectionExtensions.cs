using Backbone.AdminUi.Configuration;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;

namespace Backbone.AdminUi.Extensions;

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
