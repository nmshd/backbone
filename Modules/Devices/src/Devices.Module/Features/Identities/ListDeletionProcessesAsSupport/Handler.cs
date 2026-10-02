using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.Get(request.IdentityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var response = new Response(identity.DeletionProcesses);

        return response;
    }
}
