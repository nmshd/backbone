using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.GetFirst(Identity.HasUser(request.Username), cancellationToken);

        bool isDeleted;
        DateTime? deletionGracePeriodEndsAt;

        if (identity != null)
        {
            isDeleted = identity.IsGracePeriodOver;
            deletionGracePeriodEndsAt = identity.IsGracePeriodOver ? identity.DeletionGracePeriodEndsAt : null;
        }
        else
        {
            var auditLogEntries = await _identitiesRepository.ListIdentityDeletionProcessAuditLogs(
                IdentityDeletionProcessAuditLogEntry.IsAssociatedToUser(Username.Parse(request.Username)),
                cancellationToken);

            var deletionCompletedAuditLogEntry = auditLogEntries.FirstOrDefault(l => l.MessageKey == MessageKey.DeletionCompleted);

            isDeleted = deletionCompletedAuditLogEntry != null;
            deletionGracePeriodEndsAt = deletionCompletedAuditLogEntry?.CreatedAt;
        }

        return new Response(isDeleted, deletionGracePeriodEndsAt);
    }
}
