using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tiers.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
public class Handler : IRequestHandler<ListTiersQuery, ListTiersResponse>
{
    private readonly ITierRepository _tierRepository;

    public Handler(ITierRepository repository)
    {
        _tierRepository = repository;
    }

    public async Task<ListTiersResponse> Handle(ListTiersQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _tierRepository.FindAll(request.PaginationFilter);
        var tiersDTOList = dbPaginationResult.ItemsOnPage.Select(el =>
        {
            return new TierDTO(el.Id, el.Name);
        }).ToList();

        return new ListTiersResponse(tiersDTOList, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
