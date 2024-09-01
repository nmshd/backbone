using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;

public class DeleteRelationshipTemplatesOfIdentityCommand : IRequest
{
    public DeleteRelationshipTemplatesOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
