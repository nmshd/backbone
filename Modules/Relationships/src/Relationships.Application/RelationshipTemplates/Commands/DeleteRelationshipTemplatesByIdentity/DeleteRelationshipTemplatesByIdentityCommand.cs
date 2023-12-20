using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesByIdentity;

public class DeleteRelationshipTemplatesByIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
