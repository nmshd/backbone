using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;

public class Query : IRequest<RelationshipTemplateDTO>
{
    public required string Id { get; init; }
    public byte[]? Password { get; init; }
}
