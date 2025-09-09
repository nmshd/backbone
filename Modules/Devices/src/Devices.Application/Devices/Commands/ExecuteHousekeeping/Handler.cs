using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteDeletionProcesses(cancellationToken);
        await DeleteDeletionProcessAuditLogEntries(cancellationToken);
    }

    private async Task DeleteDeletionProcesses(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _identitiesRepository.DeleteDeletionProcesses(IdentityDeletionProcess.CanBeCleanedUp, cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "identity deletion processes", stopwatch.ElapsedMilliseconds);
    }

    private async Task DeleteDeletionProcessAuditLogEntries(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _identitiesRepository.DeleteDeletionProcessAuditLogEntries(IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp, cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "identity deletion process audit log entries", stopwatch.ElapsedMilliseconds);
    }
}
