using AutoMapper;
using Backbone.Devices.Application.Clients.DTOs;
using Backbone.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Devices.Application.Clients.Queries.ListClients;

public class Handler : IRequestHandler<ListClientsQuery, ListClientsResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly IMapper _mapper;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, IMapper mapper)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _mapper = mapper;
    }
    public async Task<ListClientsResponse> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _oAuthClientsRepository.FindAll(cancellationToken);
        var clientDtos = _mapper.Map<IEnumerable<ClientDTO>>(clients);

        return new ListClientsResponse(clientDtos);
    }
}
