using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;

public class Handler : IRequestHandler<Query, IdentityDeletionProcessDetailsDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<IdentityDeletionProcessDetailsDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(p => p.Id == request.DeletionProcessId) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
        var response = new IdentityDeletionProcessDetailsDTO(deletionProcess);

        return response;
    }
}
