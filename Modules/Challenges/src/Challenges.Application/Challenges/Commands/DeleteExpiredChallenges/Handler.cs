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
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Cancellation was request. Stopping execution...");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        var deletedChallengesCount = await _challengesRepository.DeleteExpiredChallenges(cancellationToken);

        _logger.LogInformation($"Deletion of {deletedChallengesCount} challenges successful.");

        var response = new DeleteExpiredChallengesResponse(deletedChallengesCount);

        return response;
    }
}
