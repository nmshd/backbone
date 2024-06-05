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
        var identities = await _identitiesRepository.FindAllWithAddresses(request.Addresses, cancellationToken);
        var identityDtos = identities.Select(el => new IdentitySummaryDTO(el)).ToList();

        return new ListIdentitiesResponse(identityDtos);
    }
}
