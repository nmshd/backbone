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
            DeleteExpiredChallengesLogs.CancellationRequested(_logger);
            return DeleteExpiredChallengesResponse.NoDeletedChallenges();
        }

        var deletedChallengesCount = await _challengesRepository.DeleteExpiredChallenges(cancellationToken);

        DeleteExpiredChallengesLogs.DeletionSuccessful(_logger, deletedChallengesCount);

        var response = new DeleteExpiredChallengesResponse(deletedChallengesCount);

        return response;
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> CANCELLATION_REQUESTED =
        LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(599235, "Challenges.Application.DeleteExpiredChallenges.CancellationRequested"),
            "Cancellation was requested. Stopping execution..."
        );

    private static readonly Action<ILogger, int, Exception> DELETION_SUCCESSFUL =
        LoggerMessage.Define<int>(
            LogLevel.Debug,
            new EventId(916630, "Challenges.Application.DeleteExpiredChallenges.DeletionSuccessful"),
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

internal static partial class DeleteExpiredChallengesLogs
{
    [LoggerMessage(
        EventId = 599235,
        EventName = "Challenges.Application.DeleteExpiredChallenges.CancellationRequested",
        Level = LogLevel.Debug,
        Message = "Cancellation was requested. Stopping execution...")]
    public static partial void CancellationRequested(ILogger logger);

    [LoggerMessage(
        EventId = 916630,
        EventName = "Challenges.Application.DeleteExpiredChallenges.DeletionSuccessful",
        Level = LogLevel.Debug,
        Message = "Deletion of '{deletedChallengesCount}' challenges successful.")]
    public static partial void DeletionSuccessful(ILogger logger, int deletedChallengesCount);
}
