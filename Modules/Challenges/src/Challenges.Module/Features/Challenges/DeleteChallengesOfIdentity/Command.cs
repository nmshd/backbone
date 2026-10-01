using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;

public class DeleteChallengesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
