using MediatR;

namespace Backbone.Modules.Challenges.Application.Identities.Commands.DeleteChallengeByIdentity;

public class DeleteChallengeByIdentityCommand : IRequest
{
    public string IdentityAddress { get; private set; }

    public DeleteChallengeByIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }
}
