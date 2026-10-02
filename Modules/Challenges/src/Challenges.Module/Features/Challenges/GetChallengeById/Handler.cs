using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using Backbone.Modules.Challenges.Abstractions;
using Backbone.Modules.Challenges.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;

public class Handler : IRequestHandler<Query, ChallengeDTO>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task<ChallengeDTO> Handle(Query request, CancellationToken cancellationToken)
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
