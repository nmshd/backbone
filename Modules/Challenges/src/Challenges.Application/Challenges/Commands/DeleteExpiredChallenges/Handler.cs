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
        var challenges = await _challengesRepository.FindAll(cancellationToken);

        var idsOfExpiredChallenges = challenges.Where(x => x.IsExpired()).Select(x => x.Id);

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Cancellation was request. Stopping execution...");
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        await _challengesRepository.DeleteExpiredChallenges(cancellationToken);

        _logger.LogInformation($"Deletion of {idsOfExpiredChallenges.Count()} challenges successful.");

        var response = new DeleteExpiredChallengesResponse(idsOfExpiredChallenges);

        return response;
    }
}
