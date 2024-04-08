using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Backbone.Job.IdentityDeletion;

public class CancelStaleDeletionProcessesWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger<CancelStaleDeletionProcessesWorker> _logger;
    private readonly IMediator _mediator;

    public CancelStaleDeletionProcessesWorker(IHostApplicationLifetime host,
        IMediator mediator,
        ILogger<CancelStaleDeletionProcessesWorker> logger)
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

        if (!identityDeletionProcessIds.IsNullOrEmpty())
        {
            var concatenatedIds = string.Join(", ", identityDeletionProcessIds);
            _logger.WorkerCompletedWithResults(concatenatedIds);
        }
        else
            _logger.WorkerCompletedWithoutResults();
    }
}

internal static partial class CancelIdentityDeletionProcessWorkerLogs
{
    [LoggerMessage(
        EventId = 440986,
        EventName = "Job.CancelIdentityDeletionProcessWorker.CompletedWithResults",
        Level = LogLevel.Information,
        Message = "Automatically canceled identity deletion processes: {concatenatedProcessIds}")]
    public static partial void WorkerCompletedWithResults(this ILogger logger, string concatenatedProcessIds);

    [LoggerMessage(
        EventId = 361883,
        EventName = "Job.CancelIdentityDeletionProcessWorker.CompletedWithoutResults",
        Level = LogLevel.Information,
        Message = "No identity deletion processes are pass due approval")]
    public static partial void WorkerCompletedWithoutResults(this ILogger logger);
}
