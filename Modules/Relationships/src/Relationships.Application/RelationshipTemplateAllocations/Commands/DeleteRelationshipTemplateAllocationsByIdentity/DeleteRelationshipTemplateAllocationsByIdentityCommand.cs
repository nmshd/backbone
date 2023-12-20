using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsByIdentity;
public class DeleteRelationshipTemplateAllocationsByIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
