using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
