using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
public class Handler : IRequestHandler<GetClientQuery, ClientDTO>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ITiersRepository _tierRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, ITiersRepository tiersRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _tierRepository = tiersRepository;
    }
    public async Task<ClientDTO> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        var defaultTier = await _tierRepository.FindById(client.DefaultTier, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        var clientDTO = new ClientDTO
        {
            ClientId = client.ClientId,
            DisplayName = client.DisplayName,
            DefaultTierId = defaultTier.Id,
            DefaultTierName = defaultTier.Name,
            CreatedAt = client.CreatedAt,
            MaxIdentities = client.MaxIdentities
        };

        return clientDTO;
    }
}
