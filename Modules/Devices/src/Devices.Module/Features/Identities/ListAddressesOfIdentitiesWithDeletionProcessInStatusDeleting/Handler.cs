using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<Response> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var addresses = await _identitiesRepository.ListAddressesOfIdentities(Identity.HasDeletionProcessInStatus(DeletionProcessStatus.Deleting), cancellationToken);
        return new Response(addresses);
    }
}
