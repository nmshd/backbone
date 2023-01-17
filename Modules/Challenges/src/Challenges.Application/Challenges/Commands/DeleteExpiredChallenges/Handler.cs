using Challenges.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class Handler : IRequestHandler<DeleteExpiredChallengesCommand, DeleteExpiredChallengesResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<DeleteExpiredChallengesCommand> _logger;

    public Handler(ILogger<DeleteExpiredChallengesCommand> logger, IDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<DeleteExpiredChallengesResponse> Handle(DeleteExpiredChallengesCommand request, CancellationToken cancellationToken)
    {
        var idsOfExpiredChallenges = await _dbContext
            .Set<Challenge>()
            .Where(Challenge.CanBeCleanedUp)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (idsOfExpiredChallenges.Count == 0)
        {
            _logger.LogInformation("No expired challenges found.");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        _logger.LogInformation($"Found {idsOfExpiredChallenges.Count} expired challenges. Beginning to delete...");

        _dbContext
            .Set<Challenge>()
            .RemoveRange(idsOfExpiredChallenges.Select(id => new Challenge {Id = id}));

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Cancellation was request. Stopping execution...");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Deletion of {idsOfExpiredChallenges.Count} challenges successful.");

        var response = new DeleteExpiredChallengesResponse(idsOfExpiredChallenges);

        return response;
    }
}
