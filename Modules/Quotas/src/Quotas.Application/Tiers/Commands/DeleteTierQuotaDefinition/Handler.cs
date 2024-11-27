using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;

public class Handler : IRequestHandler<DeleteTierQuotaDefinitionCommand>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(ITiersRepository tiersRepository, ILogger<Handler> logger)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteTierQuotaDefinitionCommand request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.TierId, cancellationToken, true) ?? throw new NotFoundException(nameof(Tier));

        var result = tier.DeleteQuota(request.TierQuotaDefinitionId);
        if (result.IsFailure)
            throw new DomainException(result.Error);

        await _tiersRepository.Update(tier, cancellationToken);

        _logger.DeletedTierQuotaDefinition(request.TierQuotaDefinitionId, tier.Id);
    }
}

internal static partial class DeleteTierQuotaDefinitionLogs
{
    [LoggerMessage(
        EventId = 519284,
        EventName = "Quotas.DeleteTierQuotaDefinition.DeletedTierQuotaDefinition",
        Level = LogLevel.Information,
        Message = "Successfully deleted tier quota definition with id: '{tierQuotaDefinitionId}' from tier with id '{tierId}'.")]
    public static partial void DeletedTierQuotaDefinition(this ILogger logger, string tierQuotaDefinitionId, string tierId);
}
