using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
