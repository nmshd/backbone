using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsOfIdentity;
public class DeleteRelationshipTemplateAllocationsOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
