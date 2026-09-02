using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly HousekeepingTelemetry _telemetry;

    public Handler(IIdentitiesRepository identitiesRepository, HousekeepingTelemetry telemetry)
    {
        _identitiesRepository = identitiesRepository;
        _telemetry = telemetry;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteDeletionProcesses(cancellationToken);
        await DeleteDeletionProcessAuditLogEntries(cancellationToken);
    }

    private async Task DeleteDeletionProcesses(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("identity deletion processes", ct => _identitiesRepository.DeleteDeletionProcesses(IdentityDeletionProcess.CanBeCleanedUp, ct), cancellationToken);
    }

    private async Task DeleteDeletionProcessAuditLogEntries(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("identity deletion process audit log entries",
            ct => _identitiesRepository.DeleteDeletionProcessAuditLogEntries(IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp, ct), cancellationToken);
    }
}
