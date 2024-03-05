using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;
using MediatR;

namespace Backbone.Job.IdentityDeletion;

public class CancelIdentityDeletionProcessWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger<CancelIdentityDeletionProcessWorker> _logger;
    private readonly IMediator _mediator;

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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task StartProcessing(CancellationToken cancellationToken)
    {
        var identityDeletionProcessIds = await _mediator.Send(new CancelStaleIdentityDeletionProcessesCommand(), cancellationToken);

        var concatenatedIds = string.Join(", ", identityDeletionProcessIds);

        _logger.LogInformation("Automatically canceled identity deletion processes: [{logCanceledDeletionProcessIds}]", concatenatedIds);
    }
}
