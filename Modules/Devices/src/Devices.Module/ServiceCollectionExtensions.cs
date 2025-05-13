using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Infrastructure;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Module;

public static class ServiceCollectionExtensions
{
    public static void AddDevicesModule(this IServiceCollection services, Action<DevicesConfiguration> configure)
    {
        var configuration = new DevicesConfiguration();

        configure.Invoke(configuration);

        services.Configure(configuration.Application);
        services.Configure(configuration.Infrastructure);
    }
}

public class DevicesConfiguration
{
    public ApplicationConfiguration Application { get; set; } = new();
    public InfrastructureConfiguration Infrastructure { get; set; } = new();
}
