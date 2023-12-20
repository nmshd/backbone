using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsByIdentity;
internal class Handler(IRelationshipTemplateAllocationsRepository relationshipTemplateAllocationsRepository) : IRequestHandler<DeleteRelationshipTemplateAllocationsByIdentityCommand>
{
    public async Task Handle(DeleteRelationshipTemplateAllocationsByIdentityCommand request, CancellationToken cancellationToken)
    {
        await relationshipTemplateAllocationsRepository.DeleteAllocations(RelationshipTemplateAllocation.WasAllocatedBy(request.IdentityAddress), cancellationToken);
    }
}
