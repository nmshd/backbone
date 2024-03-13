using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;

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
