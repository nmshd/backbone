using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tiers.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
public class Handler : IRequestHandler<ListTiersQuery, ListTiersResponse>
{
    private readonly ITiersRepository _tierRepository;

    public Handler(ITiersRepository repository)
    {
        _tierRepository = repository;
    }

    public async Task<ListTiersResponse> Handle(ListTiersQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _tierRepository.FindAll(request.PaginationFilter, cancellationToken);
        var tierDtos = dbPaginationResult.ItemsOnPage.Select(el => new TierDTO(el.Id, el.Name)).ToList();

        return new ListTiersResponse(tierDtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
