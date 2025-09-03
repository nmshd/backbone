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
        await DeleteDeletionProcessAuditLogEntries(cancellationToken);
    }

    private async Task DeleteDeletionProcessAuditLogEntries(CancellationToken cancellationToken)
    {
        var numberOfDeletedAuditLogEntries = await _identitiesRepository.DeleteDeletionProcessAuditLogEntries(IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp, cancellationToken);

        _logger.LogInformation("Deleted {numberOfDeletedItems} identity deletion process audit log entries", numberOfDeletedAuditLogEntries);
    }
}
