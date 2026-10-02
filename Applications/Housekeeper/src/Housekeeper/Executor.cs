using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using MediatR;
using ExecuteAnnouncementsModuleHousekeepingCommand = Backbone.Modules.Announcements.Module.Features.Announcements.ExecuteHousekeeping.Command;
using ExecuteChallengesModuleHousekeepingCommand = Backbone.Modules.Challenges.Module.Features.Challenges.ExecuteHousekeeping.Command;
using ExecuteDevicesModuleHousekeepingCommand = Backbone.Modules.Devices.Module.Features.Devices.ExecuteHousekeeping.Command;
using ExecuteFilesModuleHousekeepingCommand = Backbone.Modules.Files.Module.Features.Files.ExecuteHousekeeping.Command;
using ExecuteRelationshipsModuleHousekeepingCommand = Backbone.Modules.Relationships.Module.Features.Relationships.ExecuteHousekeeping.Command;
using ExecuteSynchronizationModuleHousekeepingCommand = Backbone.Modules.Synchronization.Module.Features.SyncRuns.ExecuteHousekeeping.Command;
using ExecuteTokensModuleHousekeepingCommand = Backbone.Modules.Tokens.Module.Features.Tokens.ExecuteHousekeeping.Command;

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
        var activity = HousekeepingTelemetry.ACTIVITY_SOURCE.StartActivity("housekeeper_run");

        if (activity == null)
            return null;

        activity.SetTag("housekeeping.trace_id", activity.TraceId.ToString());
        activity.SetTag("housekeeper.operation", "job_run");

        return activity;
    }
}
