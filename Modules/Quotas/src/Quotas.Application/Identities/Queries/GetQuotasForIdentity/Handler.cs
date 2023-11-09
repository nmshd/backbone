using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Metrics;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class Handler : IRequestHandler<ListQuotasForIdentityQuery, ListQuotasForIdentityResponse>
{
    private readonly IMemoryCache _cache;

    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public Handler(IMemoryCache cache, IIdentitiesRepository identitiesRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _cache = cache;
        _identitiesRepository = identitiesRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task<ListQuotasForIdentityResponse> Handle(ListQuotasForIdentityQuery request, CancellationToken cancellationToken)
    {
        var identityList = await _identitiesRepository.FindByAddresses(new List<string> { request.Address }.AsReadOnly(), cancellationToken, true);
        var identity = identityList.First();

        var individualQuotasForIdentityTasks = identity.IndividualQuotas
            .Select(async q =>
            {
                var calculator = _metricCalculatorFactory.CreateFor(q.MetricKey);
                var usage = await calculator.CalculateUsage(q.Period.CalculateBegin(), q.Period.CalculateEnd(), identity.Address, cancellationToken);

                return new QuotaDTO(
                    q,
                    new MetricDTO(new Metric(q.MetricKey, q.MetricKey.ToString()!)),
                    usage);
            });

        var individualQuotasForIdentity = await Task.WhenAll(individualQuotasForIdentityTasks);

        var tierQuotasForIdentityTasks = identity.TierQuotas
            .Select(async q =>
            {
                var calculator = _metricCalculatorFactory.CreateFor(q.MetricKey);
                var usage = await calculator.CalculateUsage(q.Period.CalculateBegin(), q.Period.CalculateEnd(), identity.Address, cancellationToken);

                return new QuotaDTO(
                    q,
                    new MetricDTO(new Metric(q.MetricKey, q.MetricKey.ToString()!)),
                    usage);
            })
            .ToList();

        var tierQuotasForIdentity = await Task.WhenAll(tierQuotasForIdentityTasks);

        var quotasForIdentity = individualQuotasForIdentity.Concat(tierQuotasForIdentity).ToList();

        var totalRecords = quotasForIdentity.Count;
        var pagedQuotas = PaginationHelper.ApplyPagedResponse(quotasForIdentity, request.PaginationFilter);

        var pagedResponse = new PagedResponse<QuotaDTO>(pagedQuotas, request.PaginationFilter, totalRecords);
        return new ListQuotasForIdentityResponse(pagedResponse, request.PaginationFilter, totalRecords);
    }
}

public static class PaginationHelper
{
    public static List<T> ApplyPagedResponse<T>(List<T> source, PaginationFilter paginationFilter)
    {
        var page = paginationFilter.PageNumber - 1;
        var pageSize = paginationFilter.PageSize ??= 10;

        var pagedData = source
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();

        return pagedData;
    }
}
