using AutoMapper;
using Challenges.Application.Challenges.DTOs;
using Challenges.Application.Extensions;
using Challenges.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using MediatR;

namespace Challenges.Application.Challenges.Queries.GetChallengeById;

public class Handler : IRequestHandler<GetChallengeByIdQuery, ChallengeDTO>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ChallengeDTO> Handle(GetChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _dbContext
            .SetReadOnly<Challenge>()
            .NotExpired()
            .FirstWithId(request.Id, cancellationToken);

        return _mapper.Map<ChallengeDTO>(challenge);
    }
}
