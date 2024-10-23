using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;

public class DeleteChallengesOfIdentityCommand : IRequest
{
    public DeleteChallengesOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
