using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationshipsOfIdentity;

public class Handler : IRequestHandler<DecomposeRelationshipsOfIdentityCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(DecomposeRelationshipsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationships = (await _relationshipsRepository.FindRelationships(Relationship.HasParticipant(request.IdentityAddress), cancellationToken)).ToList();

        foreach (var relationship in relationships) relationship.DecomposeDueToIdentityDeletion(request.IdentityAddress);

        await _relationshipsRepository.Update(relationships);
    }
}
