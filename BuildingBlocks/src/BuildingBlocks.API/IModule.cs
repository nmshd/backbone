using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.API;

public interface IModule
{
    string Name { get; }
    void ConfigureServices(IServiceCollection services, IConfigurationSection configuration);
    void ConfigureEventBus(IEventBus eventBus);
}
