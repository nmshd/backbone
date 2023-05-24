using AutoMapper;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Application.Quotas.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class Handler : IRequestHandler<CreateQuotaForTierCommand, TierQuotaDefinitionDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IEventBus eventBus, IMetricsRepository metricsRepository, IMapper mapper)
    {
        _tiersRepository = tierRepository;
        _logger = logger;
        _eventBus = eventBus;
        _metricsRepository = metricsRepository;
        _mapper = mapper;
    }

    public async Task<TierQuotaDefinitionDTO> Handle(CreateQuotaForTierCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateQuotaForTierCommand ...");

        var tier = await _tiersRepository.Find(request.TierId, cancellationToken);
        var metric = await _metricsRepository.Find(request.MetricKey, cancellationToken);
        var quota = tier.CreateQuota(metric, request.Max, request.Period);

        await _tiersRepository.Update(tier, cancellationToken);

        _logger.LogTrace($"Successfully created assigned Quota to Tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(tier, quota.Id));

        _logger.LogTrace($"Successfully published QuotaCreatedForTierIntegrationEvent. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        var response = _mapper.Map<TierQuotaDefinitionDTO>(quota);
        return response;
    }
}
