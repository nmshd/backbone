using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;

public class Handler : IRequestHandler<ListClientsQuery, ListClientsResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<ListClientsResponse> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = (await _oAuthClientsRepository.List(cancellationToken)).ToList();

        var clientIds = clients.Select(c => c.ClientId).ToList();
        var numberOfIdentitiesByClient = await _oAuthClientsRepository.CountIdentities(clientIds, cancellationToken);

        return new ListClientsResponse(clients, numberOfIdentitiesByClient);
    }
}
