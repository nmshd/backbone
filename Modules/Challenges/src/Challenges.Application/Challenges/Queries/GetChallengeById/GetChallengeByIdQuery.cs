using Backbone.Challenges.Application.Challenges.DTOs;
using Backbone.Challenges.Domain.Ids;
using MediatR;

namespace Backbone.Challenges.Application.Challenges.Queries.GetChallengeById;

public class GetChallengeByIdQuery : IRequest<ChallengeDTO>
{
    public ChallengeId Id { get; set; }
}
