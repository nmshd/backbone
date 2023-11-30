using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationshipCommand;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities;
public class IdentityDeleter(Mediator mediator) : IIdentityDeleter
{
    private readonly Mediator _mediator = mediator;

    public async Task Delete(IdentityAddress identityAddress)
    {


        await _mediator.Send(new DeleteRelationshipsCommand(identityAddress));
    }
}
