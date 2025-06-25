using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;

public class DeleteChallengesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
