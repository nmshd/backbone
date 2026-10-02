using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Challenges.Abstractions;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;

public class Handler : IRequestHandler<Command, ChallengeDTO>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly IUserContext _userContext;

    public Handler(IChallengesRepository challengesRepository, IUserContext userContext)
    {
        _challengesRepository = challengesRepository;
        _userContext = userContext;
    }

    public async Task<ChallengeDTO> Handle(Command request, CancellationToken cancellationToken)
    {
        var challenge = new Challenge(_userContext.GetAddressOrNull(), _userContext.GetDeviceIdOrNull());

        await _challengesRepository.Add(challenge, cancellationToken);

        var response = new ChallengeDTO(challenge);

        return response;
    }
}
