using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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
        _ = await _identityRepository.FindByAddress(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var identityDeletionProcessAuditLogEntries = await _identityRepository.GetIdentityDeletionProcessAuditLogsByAddress(request.IdentityAddress, cancellationToken);
        return new GetDeletionProcessesAuditLogsResponse(identityDeletionProcessAuditLogEntries);
    }
}
