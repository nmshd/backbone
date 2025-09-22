using System.Linq.Expressions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
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
        Expression<Func<Identity, bool>> filter = i => (request.Addresses == null || request.Addresses.Contains(i.Address)) &&
                                                       (request.Status == null || i.Status == request.Status);

        var identities = await _identitiesRepository.List(filter, cancellationToken);
        return new ListIdentitiesResponse(identities);
    }
}
