using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplatesForIdentity;

public class AnonymizeRelationshipTemplatesForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
