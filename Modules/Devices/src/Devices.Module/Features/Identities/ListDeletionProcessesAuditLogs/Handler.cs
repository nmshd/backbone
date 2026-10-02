using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var addressHash = Hasher.HashUtf8(request.IdentityAddress);

        var identityDeletionProcessAuditLogEntries = await _identityRepository.ListIdentityDeletionProcessAuditLogs(l => l.IdentityAddressHash == addressHash, cancellationToken);

        return new Response(identityDeletionProcessAuditLogEntries.OrderBy(e => e.CreatedAt));
    }
}
