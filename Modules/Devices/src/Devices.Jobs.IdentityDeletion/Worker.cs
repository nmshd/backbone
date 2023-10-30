using System.Runtime.InteropServices.JavaScript;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var eventBus = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IEventBus>();
        eventBus.Publish(new IdentityDeletionJobConfiguration());
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
