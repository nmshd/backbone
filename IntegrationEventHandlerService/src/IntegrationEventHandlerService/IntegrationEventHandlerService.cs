using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Hosting;

namespace Backbone.IntegrationEventHandlerService;
internal class IntegrationEventHandlerService : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IEnumerable<AbstractModule> _modules;

    public IntegrationEventHandlerService(IEventBus eventBus, IEnumerable<AbstractModule> modules)
    {
        _eventBus = eventBus;
        _modules = modules;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var module in _modules)
        {
            module.ConfigureEventBus(_eventBus);
        }

        _eventBus.StartConsuming();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
