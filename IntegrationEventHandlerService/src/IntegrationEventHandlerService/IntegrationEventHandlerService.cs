using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Hosting;

namespace Backbone.IntegrationEventHandlerService;
public class IntegrationEventHandlerService : IHostedService
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
        StartProcessing();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void StartProcessing()
    {
        foreach (var module in _modules)
        {
            module.ConfigureEventBus(_eventBus);
        }

        _eventBus.StartConsuming();
    }
}
