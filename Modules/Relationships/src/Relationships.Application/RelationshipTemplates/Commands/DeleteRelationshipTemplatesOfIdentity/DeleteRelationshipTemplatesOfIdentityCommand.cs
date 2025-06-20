using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;

public class DeleteRelationshipTemplatesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
