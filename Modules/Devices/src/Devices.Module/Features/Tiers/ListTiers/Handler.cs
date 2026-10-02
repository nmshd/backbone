using Backbone.Modules.Devices.Abstractions;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.ListTiers;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly ITiersRepository _tierRepository;

    public Handler(ITiersRepository repository)
    {
        _tierRepository = repository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _tierRepository.List(request.PaginationFilter, cancellationToken);
        return new Response(dbPaginationResult, request.PaginationFilter);
    }
}
