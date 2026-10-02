using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

public class Handler : IRequestHandler<DeleteRelationshipTemplatesOfIdentityCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
    }

    public async Task Handle(DeleteRelationshipTemplatesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _relationshipTemplatesRepository.Delete(RelationshipTemplate.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
