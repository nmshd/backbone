using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
public class Handler : IRequestHandler<GetClientQuery, ClientDTO>
{
    private readonly IMapper _mapper;
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IMapper mapper, IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _mapper = mapper;
    }
    public async Task<ClientDTO> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        var clientDTO = _mapper.Map<ClientDTO>(client);

        return clientDTO;
    }
}
