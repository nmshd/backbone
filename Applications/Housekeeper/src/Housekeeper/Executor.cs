using MediatR;
using ExecuteChallengesHousekeepingCommand = Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteFilesHousekeepingCommand = Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteTokensHousekeepingCommand = Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteRelationshipsHousekeepingCommand = Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteDevicesHousekeepingCommand = Backbone.Modules.Devices.Application.Devices.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;

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

        await _mediator.Send(new ExecuteChallengesHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteFilesHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteTokensHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteRelationshipsHousekeepingCommand(), cancellationToken);
        await _mediator.Send(new ExecuteDevicesHousekeepingCommand(), cancellationToken);

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
