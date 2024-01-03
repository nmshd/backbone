using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
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
        var quotas = await identity.GetAllQuotas().AsDtos(identity.Address, _metricCalculatorFactory, metrics, cancellationToken);

        // ReSharper disable once InconsistentNaming
        var quotasForIdentityDTOs = new QuotasForIdentityDTO(quotas.ToList()).Quotas;

        return new ListQuotasForIdentityResponse(quotasForIdentityDTOs);
    }
}

file static class IdentityExtensions
{
    public static IEnumerable<Quota> GetAllQuotas(this Identity identity)
    {
        var individualQuotas = identity.IndividualQuotas;
        var tierQuotas = identity.TierQuotas;

        return new List<Quota>(individualQuotas).Concat(new List<Quota>(tierQuotas)).ToList();
    }

    public static Task<QuotaDTO[]> AsDtos(this IEnumerable<Quota> quotas, IdentityAddress identityAddress, MetricCalculatorFactory metricCalculatorFactory, IEnumerable<Metric> metrics, CancellationToken cancellationToken)
    {
        var quotaDtos = quotas.Select(async q =>
        {
            var calculator = metricCalculatorFactory.CreateFor(q.MetricKey);
            var usage = await calculator.CalculateUsage(q.Period.CalculateBegin(), q.Period.CalculateEnd(), identityAddress, cancellationToken);
            var metricDto = new MetricDTO(metrics.Single(m => m.Key == q.MetricKey));
            return new QuotaDTO(q, metricDto, usage);
        });

        return Task.WhenAll(quotaDtos);
    }
}
