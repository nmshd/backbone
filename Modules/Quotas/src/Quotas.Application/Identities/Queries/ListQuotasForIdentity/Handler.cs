using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Tooling;
using MediatR;

// ReSharper disable InconsistentNaming

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class Handler : IRequestHandler<ListQuotasForIdentityQuery, ListQuotasForIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;
    private readonly IdentityAddress _identityAddress;

    public Handler(IUserContext userContext, IIdentitiesRepository identitiesRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identityAddress = userContext.GetAddress();
        _identitiesRepository = identitiesRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task<ListQuotasForIdentityResponse> Handle(ListQuotasForIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_identityAddress, cancellationToken) ??
                       throw new Exception($"Identity with Id '{_identityAddress}' not found.");
        var quotaGroupDTOs = await identity.GetAllQuotas().AsQuotaGroupDTOs(identity.Address, _metricCalculatorFactory, cancellationToken);

        return new ListQuotasForIdentityResponse(quotaGroupDTOs);
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

    public static async Task<IEnumerable<QuotaGroupDTO>> AsQuotaGroupDTOs(this IEnumerable<Quota> quotas, IdentityAddress identityAddress, MetricCalculatorFactory metricCalculatorFactory,
        CancellationToken cancellationToken)
    {
        var singleQuotaDTOs = new List<SingleQuotaDTO>();

        foreach (var quota in quotas)
        {
            var calculator = metricCalculatorFactory.CreateFor(quota.MetricKey);
            var usage = await calculator.CalculateUsage(quota.Period.CalculateBegin(SystemTime.UtcNow), quota.Period.CalculateEnd(SystemTime.UtcNow), identityAddress, cancellationToken);

            singleQuotaDTOs.Add(new SingleQuotaDTO
            {
                Source = quota is IndividualQuota ? QuotaSource.Individual : QuotaSource.Tier,
                MetricKey = quota.MetricKey.Value,
                Max = quota.Max,
                Usage = usage,
                Period = quota.Period.ToString()
            });
        }

        return singleQuotaDTOs
            .GroupBy(quota => quota.MetricKey)
            .Select(group => new QuotaGroupDTO
            {
                MetricKey = group.Key,
                Quotas = group.ToList()
            });
    }
}
