using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAuditLogs;

public class Handler : IRequestHandler<ListDeletionProcessesAuditLogsQuery, ListDeletionProcessesAuditLogsResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<ListDeletionProcessesAuditLogsResponse> Handle(ListDeletionProcessesAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var addressHash = Hasher.HashUtf8(request.IdentityAddress);

        var identityDeletionProcessAuditLogEntries = await _identityRepository.ListIdentityDeletionProcessAuditLogs(l => l.IdentityAddressHash == addressHash, cancellationToken);

        return new ListDeletionProcessesAuditLogsResponse(identityDeletionProcessAuditLogEntries.OrderBy(e => e.CreatedAt));
    }
}
