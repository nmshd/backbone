using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsOfIdentity;
internal class Handler(IRelationshipTemplateAllocationsRepository relationshipTemplateAllocationsRepository) : IRequestHandler<DeleteRelationshipTemplateAllocationsOfIdentityCommand>
{
    public async Task Handle(DeleteRelationshipTemplateAllocationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipTemplateAllocationsRepository.DeleteAllocations(RelationshipTemplateAllocation.WasAllocatedBy(request.IdentityAddress), cancellationToken);
    }
}
