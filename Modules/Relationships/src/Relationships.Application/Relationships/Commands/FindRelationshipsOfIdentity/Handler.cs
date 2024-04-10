using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class Handler : IRequestHandler<FindRelationshipsOfIdentityQuery, FindRelationshipsOfIdentityResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<FindRelationshipsOfIdentityResponse> Handle(FindRelationshipsOfIdentityQuery request, CancellationToken cancellationToken)
    {
        var relationships = await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken);

        return new FindRelationshipsOfIdentityResponse(relationships);
    }
}
