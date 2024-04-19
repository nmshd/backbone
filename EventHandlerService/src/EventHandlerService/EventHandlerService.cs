using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.EventHandlerService;

public class EventHandlerService : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IEnumerable<AbstractModule> _modules;

    public EventHandlerService(IEventBus eventBus, IEnumerable<AbstractModule> modules)
    {
        _eventBus = eventBus;
        _modules = modules;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SubscribeToEvents();
        StartConsuming();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void StartConsuming()
    {
        foreach (var module in _modules)
        {
            module.ConfigureEventBus(_eventBus);
        }
    }

    private void SubscribeToEvents()
    {
        _eventBus.StartConsuming();
    }
}
