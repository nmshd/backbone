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
        var clients = await _oAuthClientsRepository.FindAll(cancellationToken);
        var clientOverviewDtos = _mapper.Map<IEnumerable<ClientOverviewDTO>>(clients);

        return new ListClientsResponse(clientOverviewDtos);
    }
}
