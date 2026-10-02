using Backbone.Modules.Devices.Abstractions;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.ListTiers;

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
