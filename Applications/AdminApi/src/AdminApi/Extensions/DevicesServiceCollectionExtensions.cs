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
        services.AddApplication(configuration.GetSection("Application"));

        services.ConfigureAndValidate<DevicesConfiguration.InfrastructureConfiguration>(configuration.GetSection("Infrastructure").Bind);

        var infrastructureConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<DevicesConfiguration.InfrastructureConfiguration>>().Value;

        services.AddDatabase(options =>
        {
            options.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.ConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        return services;
    }
}
