using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;

public class Handler : IRequestHandler<CreateQuotaForIdentityCommand, IdentityQuotaDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger, IMetricsRepository metricsRepository)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _metricsRepository = metricsRepository;
    }

    public async Task<IdentityQuotaDTO> Handle(CreateQuotaForIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Find(request.IdentityAddress, cancellationToken, true);

        if (identity == null)
            throw new NotFoundException(nameof(Identity));

        var parseMetricKeyResult = MetricKey.Parse(request.MetricKey);

        if (parseMetricKeyResult.IsFailure)
            throw new DomainException(parseMetricKeyResult.Error);

        var metric = await _metricsRepository.Find(parseMetricKeyResult.Value, cancellationToken);

        var individualQuota = identity.CreateIndividualQuota(metric.Key, request.Max, request.Period);

        await _identitiesRepository.Update(identity, cancellationToken);

        _logger.LogTrace($"Successfully created Quota for Identity. Identity Address: '{identity.Address}'");

        var response = new IdentityQuotaDefinitionDTO(individualQuota.Id, new MetricDTO(metric), individualQuota.Max, individualQuota.Period);
        return response;
    }
}
