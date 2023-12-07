using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsByIdentity;

public class Handler(IRelationshipsRepository relationshipsRepository) : IRequestHandler<DeleteRelationshipsByIdentityCommand>
{
    public async Task Handle(DeleteRelationshipsByIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipsRepository.DeleteRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken);
    }
}
