using AutoMapper;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class Handler : IRequestHandler<GetChallengeByIdQuery, ChallengeDTO>
{
    private readonly IChallengesDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(IChallengesDbContext dbContext, IMapper mapper)
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
