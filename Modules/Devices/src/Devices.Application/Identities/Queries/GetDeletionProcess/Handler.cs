using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcess;

public class Handler : IRequestHandler<GetDeletionProcessQuery, IdentityDeletionProcessDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<IdentityDeletionProcessDTO> Handle(GetDeletionProcessQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(p => p.Id == request.Id) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
        var response = new IdentityDeletionProcessDTO(deletionProcess);

        return response;
    }
}
