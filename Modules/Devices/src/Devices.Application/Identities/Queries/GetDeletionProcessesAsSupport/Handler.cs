using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;

public class Handler : IRequestHandler<GetDeletionProcessesAsSupportQuery, GetDeletionProcessesAsSupportResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<GetDeletionProcessesAsSupportResponse> Handle(GetDeletionProcessesAsSupportQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.FindByAddress(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var processes = identity.DeletionProcesses;
        var response = new GetDeletionProcessesAsSupportResponse(processes);

        return response;
    }
}
