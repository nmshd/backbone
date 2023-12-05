using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Application.Identities.Commands.DeleteChallengeByIdentity;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Identities;
public class IdentityDeleter(IMediator mediator) : IIdentityDeleter
{
    private readonly IMediator _mediator = mediator;

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteChallengeByIdentityCommand(identityAddress));
    }
}
