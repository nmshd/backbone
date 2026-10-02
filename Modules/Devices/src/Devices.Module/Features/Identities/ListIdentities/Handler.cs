using System.Linq.Expressions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository repository)
    {
        _identitiesRepository = repository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Expression<Func<Identity, bool>> filter = i => (request.Addresses == null || request.Addresses.Contains(i.Address)) &&
                                                       (request.Status == null || i.Status == request.Status);

        var identities = await _identitiesRepository.List(filter, cancellationToken);
        return new Response(identities);
    }
}
