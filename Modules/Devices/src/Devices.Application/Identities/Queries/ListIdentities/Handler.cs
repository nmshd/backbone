using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : IRequestHandler<ListIdentitiesQuery, ListIdentitiesResponse>
{
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository repository)
    {
        _identityRepository = repository;
    }

    public async Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _identityRepository.FindAll(request.PaginationFilter);
        var identitiesDTOs = dbPaginationResult.ItemsOnPage.Select(el => new IdentitySummaryDTO(el.Address, el.ClientId, el.PublicKey, el.IdentityVersion, el.CreatedAt, el.Devices, el.TierId)).ToList();

        return new ListIdentitiesResponse(identitiesDTOs, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
