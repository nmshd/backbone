using Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessGracePeriodReminders;
using MediatR;

namespace Backbone.Job.IdentityDeletion.Workers;

public class SendGracePeriodRemindersWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly ILogger<SendGracePeriodRemindersWorker> _logger;

    public SendGracePeriodRemindersWorker(IHostApplicationLifetime host,
        IMediator mediator,
        ILogger<SendGracePeriodRemindersWorker> logger)
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
        await _mediator.Send(new SendDeletionProcessGracePeriodRemindersCommand(), cancellationToken);

        _logger.RemindersSent();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

internal static partial class SendGracePeriodRemindersWorkerLogs
{
    [LoggerMessage(
        EventId = 441001,
        EventName = "Job.SendGracePeriodRemindersWorker.RemindersSent",
        Level = LogLevel.Information,
        Message = "Deletion process approval and grace period reminders sent.")]
    public static partial void RemindersSent(this ILogger logger);
}
