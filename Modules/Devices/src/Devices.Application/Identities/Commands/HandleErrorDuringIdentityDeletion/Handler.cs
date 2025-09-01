using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleErrorDuringIdentityDeletion;

public class Handler : IRequestHandler<HandleErrorDuringIdentityDeletionCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(HandleErrorDuringIdentityDeletionCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.IdentityAddress, cancellationToken, track: true);

        if (identity == null)
        {
            // If the identity is not found, it means it was deleted, so the deletion process is deleted as well. In this case we have to create the audit log entry directly. 
            var auditLogEntry = IdentityDeletionProcessAuditLogEntry.ErrorDuringDeletion(request.IdentityAddress, request.ErrorMessage);
            await _identitiesRepository.AddDeletionProcessAuditLogEntry(auditLogEntry);
            return;
        }

        identity.HandleErrorDuringDeletion(request.ErrorMessage);

        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
