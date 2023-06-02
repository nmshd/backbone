using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using CSharpFunctionalExtensions;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListOAuthClients;
public class Handler : IRequestHandler<ListClientsQuery, ListClientsResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
    }
    public async Task<ListClientsResponse> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _oAuthClientsRepository.FindAll(request.PaginationFilter, cancellationToken);

        var clientDTOs = dbPaginationResult.ItemsOnPage.Select(x => new ClientDTO(x.ClientId, x.DisplayName)).ToList();

        return new ListClientsResponse(clientDTOs, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
