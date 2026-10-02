using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.TerminateRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("TerminateRelationshipCommand")]
public class Command : IRequest<RelationshipMetadataDTO>
{
    public required string RelationshipId { get; init; }
}
