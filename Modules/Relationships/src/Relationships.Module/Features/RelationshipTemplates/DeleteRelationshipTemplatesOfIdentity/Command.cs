using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

public class DeleteRelationshipTemplatesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
