using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class Handler : IRequestHandler<FindRelationshipsOfIdentityQuery, FindRelationshipsByIdentityResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }
    public async Task<FindRelationshipsByIdentityResponse> Handle(FindRelationshipsOfIdentityQuery request, CancellationToken cancellationToken) => new()
    {
        Relationships = await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)
    };
}
