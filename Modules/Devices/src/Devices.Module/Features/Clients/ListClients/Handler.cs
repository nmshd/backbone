using Backbone.Modules.Devices.Abstractions;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ListClients;

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
