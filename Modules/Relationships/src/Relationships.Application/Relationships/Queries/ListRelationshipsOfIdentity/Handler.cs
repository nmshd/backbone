using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;

public class Handler : IRequestHandler<ListRelationshipsOfIdentityQuery, ListRelationshipsOfIdentityResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<ListRelationshipsOfIdentityResponse> Handle(ListRelationshipsOfIdentityQuery request, CancellationToken cancellationToken)
    {
        var relationships = await _relationshipsRepository.ListWithoutContent(Relationship.HasParticipant(request.IdentityAddress), cancellationToken);

        return new ListRelationshipsOfIdentityResponse(relationships);
    }
}
