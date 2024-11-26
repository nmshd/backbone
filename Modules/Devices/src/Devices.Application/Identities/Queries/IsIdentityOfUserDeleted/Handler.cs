using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;

public class Handler : IRequestHandler<IsIdentityOfUserDeletedQuery, IsIdentityOfUserDeletedResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<IsIdentityOfUserDeletedResponse> Handle(IsIdentityOfUserDeletedQuery request, CancellationToken cancellationToken)
    {
        var auditLogEntries = await _identitiesRepository.GetIdentityDeletionProcessAuditLogs(
            IdentityDeletionProcessAuditLogEntry.BelongsToUser(Username.Parse(request.Username)),
            cancellationToken);

        var deletionCompletedAuditLogEntry = auditLogEntries.FirstOrDefault(l => l.MessageKey == MessageKey.DeletionCompleted);

        return new IsIdentityOfUserDeletedResponse(deletionCompletedAuditLogEntry != null, deletionCompletedAuditLogEntry?.CreatedAt);
    }
}
