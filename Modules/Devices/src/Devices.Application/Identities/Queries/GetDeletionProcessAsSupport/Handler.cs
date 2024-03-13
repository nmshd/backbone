using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
public class Handler : IRequestHandler<GetDeletionProcessAsSupportQuery, IdentityDeletionProcessDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<IdentityDeletionProcessDTO> Handle(GetDeletionProcessAsSupportQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(p => p.Id == request.DeletionProcessId) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
        var response = new IdentityDeletionProcessDTO(deletionProcess);

        return response;
    }
}
