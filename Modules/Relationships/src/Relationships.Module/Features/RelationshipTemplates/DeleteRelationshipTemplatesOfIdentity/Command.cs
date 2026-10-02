using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
