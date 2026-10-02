using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
