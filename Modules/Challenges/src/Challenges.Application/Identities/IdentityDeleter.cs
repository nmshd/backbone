using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Identities;
public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteChallengesOfIdentityCommand(identityAddress));
    }
}
