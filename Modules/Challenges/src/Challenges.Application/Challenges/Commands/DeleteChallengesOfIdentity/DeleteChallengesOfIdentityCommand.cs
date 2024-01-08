using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;

public class DeleteChallengesOfIdentityCommand : IRequest
{
    public DeleteChallengesOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
