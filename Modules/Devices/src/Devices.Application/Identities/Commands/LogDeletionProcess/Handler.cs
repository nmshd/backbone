using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;

public class Handler : IRequestHandler<LogDeletionProcessCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(LogDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DataDeleted(request.IdentityAddress, request.AggregateType);
        await _identitiesRepository.AddDeletionProcessAuditLogEntry(auditLogEntry);
    }
}
