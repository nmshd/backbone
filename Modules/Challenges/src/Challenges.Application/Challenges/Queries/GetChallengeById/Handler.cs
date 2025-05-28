using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class Handler : IRequestHandler<GetChallengeByIdQuery, ChallengeDTO>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task<ChallengeDTO> Handle(GetChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _challengesRepository.Get(ChallengeId.Parse(request.Id), cancellationToken);

        if (challenge.IsExpired())
        {
            throw new NotFoundException();
        }

        var response = new ChallengeDTO(challenge);

        return response;
    }
}
