using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.EventHandlerService;

public class EventHandlerService : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IEnumerable<AbstractModule> _modules;
    private readonly ILogger<EventHandlerService> _logger;

    public EventHandlerService(IEventBus eventBus, IEnumerable<AbstractModule> modules, ILogger<EventHandlerService> logger)
    {
        _eventBus = eventBus;
        _modules = modules;
        _logger = logger;
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

    private void SubscribeToEvents()
    {
        _logger.LogInformation("Subscribing to events...");
        foreach (var module in _modules)
        {
            module.ConfigureEventBus(_eventBus);
        }

        _logger.LogInformation("Successfully subscribed to events.");
    }

    private void StartConsuming()
    {
        _eventBus.StartConsuming();
    }
}
