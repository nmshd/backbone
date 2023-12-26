using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;

public class Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository) : IRequestHandler<DeleteRelationshipTemplatesOfIdentityCommand>
{
    public async Task Handle(DeleteRelationshipTemplatesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipTemplatesRepository.DeleteTemplates(RelationshipTemplate.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
