using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsOfIdentity;
public class DeleteRelationshipTemplateAllocationsOfIdentityCommand : IRequest
{
    public DeleteRelationshipTemplateAllocationsOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
