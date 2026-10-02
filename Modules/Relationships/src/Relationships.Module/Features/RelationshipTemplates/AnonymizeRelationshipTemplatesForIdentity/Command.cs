using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;

public class AnonymizeRelationshipTemplatesForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
