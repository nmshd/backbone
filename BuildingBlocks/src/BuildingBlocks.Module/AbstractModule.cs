using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Module;

public abstract class AbstractModule
{
    public abstract string Name { get; }

    public abstract void ConfigureServices(IServiceCollection services, IConfigurationSection configuration);

    public abstract Task ConfigureEventBus(IEventBus eventBus);

    public virtual void PostStartupValidation(IServiceProvider serviceProvider)
    {
    }
}
