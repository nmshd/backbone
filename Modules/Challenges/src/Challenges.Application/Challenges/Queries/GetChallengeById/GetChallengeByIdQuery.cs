using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class GetChallengeByIdQuery : IRequest<ChallengeDTO>
{
    public required ChallengeId Id { get; set; }
}
