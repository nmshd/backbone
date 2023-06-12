using AutoMapper;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
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
        var client = await _oAuthClientsRepository.FindAll(cancellationToken);

        var clientDTO = _mapper.Map<IEnumerable<ClientDTO>>(client);

        return new ListClientsResponse()
        {
            Items = clientDTO
        };
    }
}
