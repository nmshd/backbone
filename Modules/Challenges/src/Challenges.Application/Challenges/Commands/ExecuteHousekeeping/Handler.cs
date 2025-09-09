using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IChallengesRepository challengesRepository, ILogger<Handler> logger)
    {
        _challengesRepository = challengesRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteChallenges(cancellationToken);
    }

    private async Task DeleteChallenges(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _challengesRepository.Delete(Challenge.CanBeCleanedUp, cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "challenges", stopwatch.ElapsedMilliseconds);
    }
}
