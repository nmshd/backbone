using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : IRequestHandler<ListIdentitiesQuery, ListIdentitiesResponse>
{
    private readonly IIdentityRepository _identityRepository;

    public Handler(IIdentityRepository repository)
    {
        _identityRepository = repository;
    }

    public async Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _identityRepository.FindAll(request.PaginationFilter);
        var identitiesDTOList = dbPaginationResult.ItemsOnPage.Select(el =>
        {
            return new IdentitySummaryDTO(el.Address, el.ClientId, el.PublicKey, el.IdentityVersion, el.CreatedAt, el.Devices.Count);
        }).ToList();

        return new ListIdentitiesResponse(identitiesDTOList, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
