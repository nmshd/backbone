using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;
using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;
using MediatR;

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

    public async Task Execute()
    {
        _logger.StartingDeletion();
        await _mediator.Send(new DeleteExpiredChallengesCommand());
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
}
