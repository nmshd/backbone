using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class Handler : IRequestHandler<AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private IRelationshipTemplatesRepository _relationshipTemplatesRepository;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
    }

    public async Task Handle(AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand request, CancellationToken cancellationToken)
    {
        var allocations = await _relationshipTemplatesRepository.FindRelationshipTemplateAllocations(RelationshipTemplateAllocation.WasAllocatedBy(request.IdentityAddress), cancellationToken);
        var updatedAllocations = new List<RelationshipTemplateAllocation>();

        foreach (var allocation in allocations)
        {
            if (allocation.ReplaceIdentityAddress(request.IdentityAddress, DELETED_IDENTITY_STRING))
                updatedAllocations.Add(allocation);
        }

        await _relationshipTemplatesRepository.UpdateRelationshipTemplateAllocations(updatedAllocations, cancellationToken);
    }
}
