using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class Handler : IRequestHandler<DeleteExpiredChallengesCommand, DeleteExpiredChallengesResponse>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly ILogger<DeleteExpiredChallengesCommand> _logger;

    public Handler(ILogger<DeleteExpiredChallengesCommand> logger, IChallengesRepository challengesRepository)
    {
        _logger = logger;
        _challengesRepository = challengesRepository;
    }

    public async Task<DeleteExpiredChallengesResponse> Handle(DeleteExpiredChallengesCommand request, CancellationToken cancellationToken)
    {
        var idsOfExpiredChallenges = await _challengesRepository.FindExpiredChallenges(cancellationToken);

        if (idsOfExpiredChallenges.Count == 0)
        {
            _logger.LogInformation("No expired challenges found.");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        _logger.LogInformation($"Found {idsOfExpiredChallenges.Count} expired challenges. Beginning to delete...");
       
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Cancellation was request. Stopping execution...");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        await _challengesRepository.DeleteExpiredChallenges(idsOfExpiredChallenges, cancellationToken);

        _logger.LogInformation($"Deletion of {idsOfExpiredChallenges.Count} challenges successful.");

        var response = new DeleteExpiredChallengesResponse(idsOfExpiredChallenges);

        return response;
    }
}
