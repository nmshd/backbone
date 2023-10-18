using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
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
            _logger.CancellationRequested();
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        var deletedChallengesCount = await _challengesRepository.DeleteExpiredChallenges(cancellationToken);

        _logger.DeletionSuccessful(deletedChallengesCount);

        var response = new DeleteExpiredChallengesResponse(deletedChallengesCount);

        return response;
    }
}

internal static partial class DeleteExpiredChallengesLogs
{
    [LoggerMessage(
        EventId = 599235,
        EventName = "Challenges.Application.DeleteExpiredChallenges.CancellationRequested",
        Level = LogLevel.Debug,
        Message = "Cancellation was requested. Stopping execution...")]
    public static partial void CancellationRequested(this ILogger logger);

    [LoggerMessage(
        EventId = 916630,
        EventName = "Challenges.Application.DeleteExpiredChallenges.DeletionSuccessful",
        Level = LogLevel.Debug,
        Message = "Deletion of '{deletedChallengesCount}' challenges successful.")]
    public static partial void DeletionSuccessful(this ILogger logger, int deletedChallengesCount);
}
