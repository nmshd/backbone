using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;

public class Handler : IRequestHandler<ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingQuery, ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse> Handle(ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingQuery request,
        CancellationToken cancellationToken)
    {
        var addresses = await _identitiesRepository.ListAddressesOfIdentities(Identity.HasDeletionProcessInStatus(DeletionProcessStatus.Deleting), cancellationToken);
        return new ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse(addresses);
    }
}
