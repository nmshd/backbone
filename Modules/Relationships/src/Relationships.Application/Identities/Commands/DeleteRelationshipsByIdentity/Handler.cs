using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationshipsByIdentity;

public class Handler(IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository) : IRequestHandler<DeleteRelationshipsByIdentityCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository = relationshipsRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository = relationshipTemplatesRepository;

    public async Task Handle(DeleteRelationshipsByIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationships = await _relationshipsRepository.FindRelationshipsWithIdentityAddress(request.IdentityAddress, cancellationToken);
        await _relationshipsRepository.Delete(relationships.Select(r => r.Id), cancellationToken);

        var relationshipTemplates = await _relationshipTemplatesRepository.FindTemplatesCreatedByIdentityAddress(request.IdentityAddress, cancellationToken);
        await _relationshipTemplatesRepository.Delete(relationshipTemplates.Select(r => r.Id), cancellationToken);
    }
}
