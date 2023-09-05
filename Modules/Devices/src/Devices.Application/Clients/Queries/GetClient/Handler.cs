using AutoMapper;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.OpenIddict;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
public class Handler : IRequestHandler<GetClientQuery, ClientDTO>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly IMapper _mapper;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, IMapper mapper)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _mapper = mapper;
    }
    public async Task<ClientDTO> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(CustomOpenIddictEntityFrameworkCoreApplication));
        var clientDto = _mapper.Map<ClientDTO>(new OAuthClient(client.ClientId!, client.DisplayName!, client.TierId));

        return clientDto;
    }
}
