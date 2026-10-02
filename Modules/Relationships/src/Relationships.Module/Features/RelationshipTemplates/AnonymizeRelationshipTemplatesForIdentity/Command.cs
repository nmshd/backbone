using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AnonymizeRelationshipTemplatesForIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
