using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsOfIdentity;

public class Handler(IRelationshipsRepository relationshipsRepository) : IRequestHandler<DeleteRelationshipsOfIdentityCommand>
{
    public async Task Handle(DeleteRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipsRepository.DeleteRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken);
    }
}
