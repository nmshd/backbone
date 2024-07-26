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
        await _identitiesRepository.AddDeletionProcessAuditLogEntry(IdentityDeletionProcessAuditLogEntry.DataDeleted(request.IdentityAddress, request.AggregateType.ToString()));
    }
}
