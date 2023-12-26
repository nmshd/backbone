using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class Handler(IRelationshipsRepository relationshipsRepository) : IRequestHandler<FindRelationshipsOfIdentityCommand, FindRelationshipsByIdentityResponse>
{
    public async Task<FindRelationshipsByIdentityResponse> Handle(FindRelationshipsOfIdentityCommand request, CancellationToken cancellationToken) => new()
    {
        Relationships = await relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)
    };
}
