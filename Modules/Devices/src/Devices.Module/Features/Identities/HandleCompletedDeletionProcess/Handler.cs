using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleCompletedDeletionProcess;

public class Handler : IRequestHandler<Command>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _identitiesRepository.AddDeletionProcessAuditLogEntry(IdentityDeletionProcessAuditLogEntry.DeletionCompleted(request.IdentityAddress));

        await AssociateUsernames(request, cancellationToken);
    }

    private async Task AssociateUsernames(Command request, CancellationToken cancellationToken)
    {
        var identityAddressHash = Hasher.HashUtf8(request.IdentityAddress);

        var auditLogEntries = await _identitiesRepository.ListIdentityDeletionProcessAuditLogs(l => l.IdentityAddressHash == identityAddressHash, CancellationToken.None, track: true);

        var auditLogEntriesArray = auditLogEntries.ToArray();

        foreach (var auditLogEntry in auditLogEntriesArray)
        {
            auditLogEntry.AssociateUsernames(request.Usernames.Select(Username.Parse));
        }

        await _identitiesRepository.Update(auditLogEntriesArray, cancellationToken);
    }
}
