using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplatesForIdentity;

public class AnonymizeRelationshipTemplatesForIdentityCommand : IRequest
{
    public AnonymizeRelationshipTemplatesForIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
