using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class Handler : IRequestHandler<AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand request, CancellationToken cancellationToken)
    {
        var allocations = await _relationshipsRepository.FindRelationshipTemplateAllocations(RelationshipTemplateAllocation.WasAllocatedBy(request.IdentityAddress), cancellationToken);
        foreach (var allocation in allocations)
        {
            if (allocation.ReplaceIdentityAddress(request.IdentityAddress, DELETED_IDENTITY_STRING))
            {
                await _relationshipsRepository.UpdateRelationshipTemplateAllocation(allocation, cancellationToken);
            }
        }
    }
}
