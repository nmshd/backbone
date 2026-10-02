using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsOwner;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identityRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identityRepository, IUserContext userContext)
    {
        _identityRepository = identityRepository;
        _userContext = userContext;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.Get(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var processes = identity.DeletionProcesses;
        var response = new Response(processes);

        return response;
    }
}
