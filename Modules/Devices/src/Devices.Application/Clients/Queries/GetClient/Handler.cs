using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;

public class Handler : IRequestHandler<GetClientQuery, ClientDTO>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, IIdentitiesRepository identitiesRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<ClientDTO> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Get(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        var numberOfIdentities = await _identitiesRepository.CountByClientId(client.ClientId, cancellationToken);

        return new ClientDTO(client, numberOfIdentities);
    }
}
