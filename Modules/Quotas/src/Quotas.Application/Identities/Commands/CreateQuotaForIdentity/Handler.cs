using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.Extensions.Logging;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;

public class Handler : IRequestHandler<CreateQuotaForIdentityCommand, IndividualQuotaDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IMetricsRepository _metricsRepository;
    private readonly IMetricStatusesService _metricStatusesService;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger, IMetricsRepository metricsRepository, IMetricStatusesService metricStatusesService)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _metricsRepository = metricsRepository;
        _metricStatusesService = metricStatusesService;
    }

    public async Task<IndividualQuotaDTO> Handle(CreateQuotaForIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        var parseMetricKeyResult = MetricKey.Parse(request.MetricKey);

        if (parseMetricKeyResult.IsFailure)
            throw new DomainException(parseMetricKeyResult.Error);

        var metric = await _metricsRepository.Get(parseMetricKeyResult.Value, cancellationToken);

        var individualQuota = identity.CreateIndividualQuota(metric.Key, request.Max, request.Period);

        await _identitiesRepository.Update(identity, cancellationToken);

        _logger.CreatedQuotaForIdentities(individualQuota.Id);

        var identityAddresses = new List<string> { identity.Address };
        var metrics = new List<MetricKey> { metric.Key };
        await _metricStatusesService.RecalculateMetricStatuses(identityAddresses, metrics, MetricUpdateType.All, cancellationToken);

        var response = new IndividualQuotaDTO(individualQuota.Id, new MetricDTO(metric), individualQuota.Max, individualQuota.Period);
        return response;
    }
}

internal static partial class CreateQuotaForIdentityLogs
{
    [LoggerMessage(
        EventId = 868289,
        EventName = "Quotas.CreateQuotaForIdentity.CreatedQuotaForIdentities",
        Level = LogLevel.Information,
        Message = "Successfully created Quota for the identity. Quota ID: '{quotaId}'.")]
    public static partial void CreatedQuotaForIdentities(this ILogger logger, string quotaId);
}
