using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand : IRequest
{
    public AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
