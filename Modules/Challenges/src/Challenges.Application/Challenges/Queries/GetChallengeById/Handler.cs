﻿using AutoMapper;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class Handler : IRequestHandler<GetChallengeByIdQuery, ChallengeDTO>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly IMapper _mapper;

    public Handler(IChallengesRepository challengesRepository, IMapper mapper)
    {
        _challengesRepository = challengesRepository;
        _mapper = mapper;
    }

    public async Task<ChallengeDTO> Handle(GetChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _challengesRepository.Find(request.Id, cancellationToken);
       
        await _challengesRepository.Update(challenge, cancellationToken);

        var response = _mapper.Map<ChallengeDTO>(challenge);

        return response;
    }
}
