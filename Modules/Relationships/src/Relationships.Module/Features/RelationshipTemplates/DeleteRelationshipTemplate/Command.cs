using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplate;

public class Command : IRequest
{
    public required string Id { get; init; }
}
