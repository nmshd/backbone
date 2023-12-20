using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsByIdentity;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesByIdentity;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{
    private readonly IMediator _mediator = mediator;

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteRelationshipsByIdentityCommand(identityAddress));
        await _mediator.Send(new DeleteRelationshipTemplatesByIdentityCommand(identityAddress));
    }
}
