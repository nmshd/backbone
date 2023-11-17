using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using MediatR;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
        using var scope = _serviceScopeFactory.CreateScope();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new UpdateDeletionProcessesCommand(), cancellationToken);
        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
