using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class GetChallengeByIdQuery : IRequest<ChallengeDTO>
{
    public required string Id { get; init; }
}
