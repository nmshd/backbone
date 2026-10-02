using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.TerminateRelationship;

public class Command : IRequest<RelationshipMetadataDTO>
{
    public required string RelationshipId { get; init; }
}
