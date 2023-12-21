using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsByIdentity;

public class Handler(IRelationshipsRepository relationshipsRepository) : IRequestHandler<FindRelationshipsByIdentityCommand, FindRelationshipsByIdentityResponse>
{
    public async Task<FindRelationshipsByIdentityResponse> Handle(FindRelationshipsByIdentityCommand request, CancellationToken cancellationToken) => new()
    {
        Relationships = await relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)
    };
}
