using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : IRequestHandler<ListIdentitiesQuery, ListIdentitiesResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository repository)
    {
        _identitiesRepository = repository;
    }

    public async Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _identitiesRepository.FindAll(request.PaginationFilter);
        var identityDTOs = dbPaginationResult.ItemsOnPage.Select(el => new IdentitySummaryDTO(el.Address, el.ClientId, el.PublicKey, el.IdentityVersion, el.CreatedAt, el.Devices, el.TierId)).ToList();

        return new ListIdentitiesResponse(identityDTOs, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
