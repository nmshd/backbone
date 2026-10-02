using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteRelationshipTemplatesOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
