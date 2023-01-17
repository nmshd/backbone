using Challenges.Application.Challenges.DTOs;
using Challenges.Domain.Ids;
using MediatR;

namespace Challenges.Application.Challenges.Queries.GetChallengeById;

public class GetChallengeByIdQuery : IRequest<ChallengeDTO>
{
    public ChallengeId Id { get; set; }
}
