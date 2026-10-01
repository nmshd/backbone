using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;

public class GetChallengeByIdQuery : IRequest<ChallengeDTO>
{
    public required string Id { get; init; }
}
