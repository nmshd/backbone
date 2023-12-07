using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipTemplatesByIdentity;

public class Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository) : IRequestHandler<DeleteRelationshipTemplatesByIdentityCommand>
{
    public async Task Handle(DeleteRelationshipTemplatesByIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipTemplatesRepository.DeleteTemplates(RelationshipTemplate.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
