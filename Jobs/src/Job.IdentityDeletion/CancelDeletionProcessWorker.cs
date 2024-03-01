using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
//using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Job.IdentityDeletion;
public class CancelDeletionProcessWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly ILogger<CancelDeletionProcessWorker> _logger;

    public CancelDeletionProcessWorker(IHostApplicationLifetime host,
        IMediator mediator,
        ILogger<CancelDeletionProcessWorker> logger)
    {
        _host = host;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartProcessing(cancellationToken);

        _host.StopApplication();
    }

    private async Task StartProcessing(CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelStaleDeletionProcessesCommand(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
