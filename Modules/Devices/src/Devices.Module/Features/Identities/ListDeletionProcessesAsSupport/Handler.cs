using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

public class Handler : IRequestHandler<ListDeletionProcessesAsSupportQuery, GetDeletionProcessesAsSupportResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<GetDeletionProcessesAsSupportResponse> Handle(ListDeletionProcessesAsSupportQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.Get(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var response = new GetDeletionProcessesAsSupportResponse(identity.DeletionProcesses);

        return response;
    }
}
