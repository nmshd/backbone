using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
