using AutoMapper;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence;
using Backbone.Modules.Challenges.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.CreateChallenge;

public class Handler : IRequestHandler<CreateChallengeCommand, ChallengeDTO>
{
    private readonly IChallengesDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IChallengesDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChallengeDTO> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = new Challenge(_userContext.GetAddressOrNull(), _userContext.GetDeviceIdOrNull());
        await _dbContext.Set<Challenge>().AddAsync(challenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ChallengeDTO>(challenge);
    }
}
