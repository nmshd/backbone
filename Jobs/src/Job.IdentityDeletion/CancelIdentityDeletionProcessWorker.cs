using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using MediatR;

namespace Backbone.Job.IdentityDeletion;
public class CancelIdentityDeletionProcessWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly ILogger<CancelIdentityDeletionProcessWorker> _logger;

    public CancelIdentityDeletionProcessWorker(IHostApplicationLifetime host,
        IMediator mediator,
        ILogger<CancelIdentityDeletionProcessWorker> logger)
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
        var identityDeletionProcessIds = await _mediator.Send(new CancelStaleDeletionProcessesCommand(), cancellationToken);

        var logCanceledDeletionProcessIds = string.Join(", ", identityDeletionProcessIds.CanceledIdentityDeletionPrecessIds);

        _logger.LogInformation("Automatically canceled identity deletion processes: [{logCanceledDeletionProcessIds}]", logCanceledDeletionProcessIds);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
