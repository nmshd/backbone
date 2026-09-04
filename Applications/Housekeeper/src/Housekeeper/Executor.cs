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

    public Executor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        using var activity = StartHousekeeperActivity();

        try
        {
            await HousekeepingTelemetry.TrackModuleDeletion("Announcements", ct => _mediator.Send(new ExecuteAnnouncementsModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Challenges", ct => _mediator.Send(new ExecuteChallengesModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Devices", ct => _mediator.Send(new ExecuteDevicesModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Files", ct => _mediator.Send(new ExecuteFilesModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Relationships", ct => _mediator.Send(new ExecuteRelationshipsModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Synchronization", ct => _mediator.Send(new ExecuteSynchronizationModuleHousekeepingCommand(), ct), cancellationToken);
            await HousekeepingTelemetry.TrackModuleDeletion("Tokens", ct => _mediator.Send(new ExecuteTokensModuleHousekeepingCommand(), ct), cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    private static Activity? StartHousekeeperActivity()
    {
        // ReSharper disable once ExplicitCallerInfoArgument
        var activity = HousekeepingDiagnostics.ACTIVITY_SOURCE.StartActivity("housekeeper_run");

        if (activity == null)
            return null;

        activity.SetTag("housekeeper.operation", "job_run");

        return activity;
    }
}
