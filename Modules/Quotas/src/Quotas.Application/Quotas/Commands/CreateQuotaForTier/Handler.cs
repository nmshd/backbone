using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class Handler : IRequestHandler<CreateQuotaForTierCommand, CreateQuotaForTierResponse>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IEventBus eventBus)
    {
        _tierRepository = tierRepository;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<CreateQuotaForTierResponse> Handle(CreateQuotaForTierCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateQuotaForTierCommand ...");

        var tier = await _tierRepository.Find(request.TierId, cancellationToken);
        var quota = tier.CreateQuota(request.Metric, request.Max, request.Period);

        await _tierRepository.Update(tier, cancellationToken);

        _logger.LogTrace($"Successfully created assigned Quota to Tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(tier, quota));

        _logger.LogTrace($"Successfully published QuotaCreatedForTierIntegrationEvent. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        var response = new CreateQuotaForTierResponse(quota.Id, quota.Metric, quota.Max, quota.Period);
        return response;
    }
}
