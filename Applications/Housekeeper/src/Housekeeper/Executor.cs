using MediatR;
using ExecuteAnnouncementsModuleHousekeepingCommand = Backbone.Modules.Announcements.Application.Announcements.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteChallengesModuleHousekeepingCommand = Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteFilesModuleHousekeepingCommand = Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteTokensModuleHousekeepingCommand = Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteRelationshipsModuleHousekeepingCommand = Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteDevicesModuleHousekeepingCommand = Backbone.Modules.Devices.Application.Devices.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteSynchronizationModuleHousekeepingCommand = Backbone.Modules.Synchronization.Application.SyncRuns.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;

namespace Backbone.Housekeeper;

public class Executor
{
    private readonly IMediator _mediator;
    private readonly ILogger<Executor> _logger;

    public Executor(IMediator mediator, ILogger<Executor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _logger.StartingDeletion();

        await _mediator.Send(new ExecuteAnnouncementsModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteChallengesModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteDevicesModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteFilesModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteRelationshipsModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteSynchronizationModuleHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteTokensModuleHousekeepingCommand(), cancellationToken);

        _logger.FinishedDeletion();
    }
}

internal static partial class ExecutorLogs
{
    [LoggerMessage(
        EventId = 468524,
        EventName = "Housekeeper.Executor.StartingDeletion",
        Level = LogLevel.Information,
        Message = "Starting deletion...")]
    public static partial void StartingDeletion(this ILogger logger);

    [LoggerMessage(
        EventId = 864565,
        EventName = "Housekeeper.Executor.ResultItem",
        Level = LogLevel.Information,
        Message = "Deleted {count} items of type '{itemType}'.")]
    public static partial void LogResult(this ILogger logger, int count, string itemType);

    [LoggerMessage(
        EventId = 945132,
        EventName = "Housekeeper.Executor.FinishedDeletion",
        Level = LogLevel.Information,
        Message = "Finished deletion")]
    public static partial void FinishedDeletion(this ILogger logger);
}
