using AutoMapper;
using Challenges.Application.Challenges.DTOs;
using Challenges.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Challenges.Application.Challenges.Commands.CreateChallenge;

public class Handler : IRequestHandler<CreateChallengeCommand, ChallengeDTO>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper)
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
