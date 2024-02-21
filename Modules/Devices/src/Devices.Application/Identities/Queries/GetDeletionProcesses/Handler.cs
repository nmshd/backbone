using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcesses;

public class Handler : IRequestHandler<GetDeletionProcessesQuery, GetDeletionProcessesResponse>
{
    private readonly IIdentitiesRepository _identityRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identityRepository, IUserContext userContext)
    {
        _identityRepository = identityRepository;
        _userContext = userContext;
    }

    public async Task<GetDeletionProcessesResponse> Handle(GetDeletionProcessesQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.FindByAddress(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var response = new GetDeletionProcessesResponse(identity);

        return response;
    }
}
