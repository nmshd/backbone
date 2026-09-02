using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using MediatR;
using ExecuteAnnouncementsModuleHousekeepingCommand = Backbone.Modules.Announcements.Application.Announcements.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteChallengesModuleHousekeepingCommand = Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteDevicesModuleHousekeepingCommand = Backbone.Modules.Devices.Application.Devices.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteFilesModuleHousekeepingCommand = Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteRelationshipsModuleHousekeepingCommand = Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteSynchronizationModuleHousekeepingCommand = Backbone.Modules.Synchronization.Application.SyncRuns.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;
using ExecuteTokensModuleHousekeepingCommand = Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping.ExecuteHousekeepingCommand;

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
        using var activity = StartHousekeeperActivity();

        _logger.StartingDeletion();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _mediator.Send(new ExecuteAnnouncementsModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteChallengesModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteDevicesModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteFilesModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteRelationshipsModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteSynchronizationModuleHousekeepingCommand(), cancellationToken);
            await _mediator.Send(new ExecuteTokensModuleHousekeepingCommand(), cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        stopwatch.Stop();

        _logger.DeletionCompleted(stopwatch.ElapsedMilliseconds);
    }

    private static Activity? StartHousekeeperActivity()
    {
        var activity = HousekeepingDiagnostics.ACTIVITY_SOURCE.StartActivity("housekeeper run", ActivityKind.Internal);

        if (activity == null)
            return null;

        activity.SetTag("housekeeper.operation", "run");

        return activity;
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
        EventId = 945132,
        EventName = "Housekeeper.Executor.DeletionCompleted",
        Level = LogLevel.Information,
        Message = "Deletion completed after {elapsedMilliseconds}ms.")]
    public static partial void DeletionCompleted(this ILogger logger, long elapsedMilliseconds);
}
