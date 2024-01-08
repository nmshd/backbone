using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Application.RelationshipTemplateAllocations.Commands.DeleteRelationshipTemplateAllocationsOfIdentity;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities;
public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteRelationshipsOfIdentityCommand(identityAddress));
        await _mediator.Send(new DeleteRelationshipTemplatesOfIdentityCommand(identityAddress));
        await _mediator.Send(new DeleteRelationshipTemplateAllocationsOfIdentityCommand(identityAddress));
    }
}
