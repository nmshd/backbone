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
            _logger.CancellationRequested();
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        var deletedChallengesCount = await _challengesRepository.DeleteExpiredChallenges(cancellationToken);

        _logger.DeletionSuccessful(deletedChallengesCount);

        var response = new DeleteExpiredChallengesResponse(deletedChallengesCount);

        return response;
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> CANCELLATION_REQUESTED =
        LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(599235, "Enmeshed.Challenges.Application.DeleteExpiredChallenges.Handler.CancellationRequested"),
            "Cancellation was requested. Stopping execution..."
        );

    private static readonly Action<ILogger, int, Exception> DELETION_SUCCESSFUL =
        LoggerMessage.Define<int>(
            LogLevel.Debug,
            new EventId(916630, ".DeleteExpiredChallenges.Handler.DeletionSuccessful"),
            "Deletion of '{deletedChallengesCount}' challenges successful."
        );

    public static void CancellationRequested(this ILogger logger)
    {
        CANCELLATION_REQUESTED(logger, default!);
    }

    public static void DeletionSuccessful(this ILogger logger, int numberOfDeletedChallenges)
    {
        DELETION_SUCCESSFUL(logger, numberOfDeletedChallenges, default!);
    }
}
