using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensByIdentity;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{
    private readonly IMediator _mediator = mediator;
    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteTokensByIdentityCommand(identityAddress));
    }
}
