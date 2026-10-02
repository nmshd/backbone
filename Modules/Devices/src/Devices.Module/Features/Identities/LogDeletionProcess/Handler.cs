using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;

public class Handler : IRequestHandler<Command>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DataDeleted(request.IdentityAddress, request.AggregateType);
        await _identitiesRepository.AddDeletionProcessAuditLogEntry(auditLogEntry);
    }
}
