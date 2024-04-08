using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.CreateChallenge;

public class Handler : IRequestHandler<CreateChallengeCommand, ChallengeDTO>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IChallengesRepository challengesRepository, IUserContext userContext, IMapper mapper)
    {
        _challengesRepository = challengesRepository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChallengeDTO> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = new Challenge(_userContext.GetAddressOrNull(), _userContext.GetDeviceIdOrNull());

        await _challengesRepository.Add(challenge, cancellationToken);

        var response = _mapper.Map<ChallengeDTO>(challenge);

        return response;
    }
}
