using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class Handler : IRequestHandler<ListQuotasForIdentityQuery, ListQuotasForIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;
    private readonly IMetricsRepository _metricsRepository;
    private readonly IdentityAddress _identityAddress;

    public Handler(IUserContext userContext, IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identityAddress = userContext.GetAddress();
        _identitiesRepository = identitiesRepository;
        _metricsRepository = metricsRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task<ListQuotasForIdentityResponse> Handle(ListQuotasForIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Find(_identityAddress, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        var metrics = await _metricsRepository.FindAll(cancellationToken);

        var individualQuotasForIdentityTasks = identity.IndividualQuotas
            .Select(async q =>
            {
                var calculator = _metricCalculatorFactory.CreateFor(q.MetricKey);
                var usage = await calculator.CalculateUsage(q.Period.CalculateBegin(), q.Period.CalculateEnd(), identity.Address, cancellationToken);

                var metric = new MetricDTO(metrics.Single(m => m.Key == q.MetricKey));

                return new QuotaDTO(q, metric, usage);
            });

        var individualQuotasForIdentity = await Task.WhenAll(individualQuotasForIdentityTasks);

        var tierQuotasForIdentityTasks = identity.TierQuotas
            .Select(async q =>
            {
                var calculator = _metricCalculatorFactory.CreateFor(q.MetricKey);
                var usage = await calculator.CalculateUsage(q.Period.CalculateBegin(), q.Period.CalculateEnd(), identity.Address, cancellationToken);

                var metric = new MetricDTO(metrics.Single(m => m.Key == q.MetricKey));

                return new QuotaDTO(q, metric, usage);
            });

        var tierQuotasForIdentity = await Task.WhenAll(tierQuotasForIdentityTasks);

        var quotasForIdentityDTOs = new QuotasForIdentityDTO(individualQuotasForIdentity.Concat(tierQuotasForIdentity).ToList()).Quotas;

        return new ListQuotasForIdentityResponse(quotasForIdentityDTOs);
    }
}
