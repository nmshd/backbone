using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand;
public class AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand:IRequest
{
    public AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
