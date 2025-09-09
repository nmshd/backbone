using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Module;

public interface IEventBusConfigurator
{
    Task ConfigureEventBus(IEventBus eventBus);
}

public abstract class AbstractModule<TApplicationConfiguration, TInfrastructureConfiguration> : IEventBusConfigurator
    where TApplicationConfiguration : class
    where TInfrastructureConfiguration : class
{
    public abstract string Name { get; }

    public void ConfigureServices(IServiceCollection services, IConfigurationSection moduleConfig, IConfigurationSection defaultConfig)
    {
        services.ConfigureAndValidate<TApplicationConfiguration>(options =>
        {
            defaultConfig.GetSection("Application").Bind(options);
            moduleConfig.GetSection("Application").Bind(options);
        });
        services.ConfigureAndValidate<TInfrastructureConfiguration>(options =>
        {
            defaultConfig.GetSection("Infrastructure").Bind(options);
            moduleConfig.GetSection("Infrastructure").Bind(options);
        });

        var infrastructureConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<TInfrastructureConfiguration>>().Value;

        ConfigureServices(services, infrastructureConfiguration, moduleConfig);
    }

    protected abstract void ConfigureServices(IServiceCollection services, TInfrastructureConfiguration infrastructureConfiguration, IConfigurationSection rawModuleConfiguration);

    public virtual Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
