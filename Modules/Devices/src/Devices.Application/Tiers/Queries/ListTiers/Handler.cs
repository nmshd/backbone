using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
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
        var dbPaginationResult = await _tierRepository.List(request.PaginationFilter, cancellationToken);
        return new ListTiersResponse(dbPaginationResult, request.PaginationFilter);
    }
}
