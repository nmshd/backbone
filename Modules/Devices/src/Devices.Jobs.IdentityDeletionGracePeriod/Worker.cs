using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using MediatR;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletionGracePeriod;
public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await StartProcessing(mediator, cancellationToken);

        _host.StopApplication();
    }

    public static async Task StartProcessing(IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletionProcessGracePeriodCommand(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
