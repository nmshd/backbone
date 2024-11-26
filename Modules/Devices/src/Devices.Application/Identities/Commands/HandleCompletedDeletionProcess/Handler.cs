using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;

public class Handler : IRequestHandler<HandleCompletedDeletionProcessCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(HandleCompletedDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        await _identitiesRepository.AddDeletionProcessAuditLogEntry(IdentityDeletionProcessAuditLogEntry.DeletionCompleted(request.IdentityAddress));

        await AssociateUsernames(request, cancellationToken);
    }

    private async Task AssociateUsernames(HandleCompletedDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identityAddressHash = Hasher.HashUtf8(request.IdentityAddress);

        var auditLogEntries = await _identitiesRepository.GetIdentityDeletionProcessAuditLogs(l => l.IdentityAddressHash == identityAddressHash, CancellationToken.None, track: true);

        var auditLogEntriesArray = auditLogEntries.ToArray();

        foreach (var auditLogEntry in auditLogEntriesArray)
        {
            auditLogEntry.AssociateUsernames(request.Usernames.Select(Username.Parse));
        }

        await _identitiesRepository.Update(auditLogEntriesArray, cancellationToken);
    }
}
