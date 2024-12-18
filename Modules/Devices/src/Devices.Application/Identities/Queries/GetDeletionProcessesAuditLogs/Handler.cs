using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;

public class Handler : IRequestHandler<GetDeletionProcessesAuditLogsQuery, GetDeletionProcessesAuditLogsResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<GetDeletionProcessesAuditLogsResponse> Handle(GetDeletionProcessesAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var addressHash = Hasher.HashUtf8(request.IdentityAddress);

        var identityDeletionProcessAuditLogEntries = await _identityRepository.GetIdentityDeletionProcessAuditLogs(l => l.IdentityAddressHash == addressHash, cancellationToken);

        return new GetDeletionProcessesAuditLogsResponse(identityDeletionProcessAuditLogEntries.OrderBy(e => e.CreatedAt));
    }
}
