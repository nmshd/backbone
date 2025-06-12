using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
