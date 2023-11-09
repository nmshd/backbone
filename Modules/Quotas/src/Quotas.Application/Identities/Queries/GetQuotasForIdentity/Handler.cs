using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class Handler : IRequestHandler<ListQuotasForIdentityQuery, ListQuotasForIdentityResponse>
{
    private readonly IMemoryCache _cache;

    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IMemoryCache cache, IIdentitiesRepository identitiesRepository)
    {
        _cache = cache;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<ListQuotasForIdentityResponse> Handle(ListQuotasForIdentityQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"QuotasForIdentity_{request.Address}";

        if (!_cache.TryGetValue(cacheKey, out List<QuotaDTO> cachedQuotas))
        {
            var identityList = await _identitiesRepository.FindByAddresses(new List<string> { request.Address }.AsReadOnly(), cancellationToken, true);
            var identity = identityList.First();

            var individualQuotasForIdentity = identity.IndividualQuotas
                .Select(q => new QuotaDTO(
                    q.Id,
                    QuotaSource.Individual,
                    new MetricDTO(new Metric(q.MetricKey, q.MetricKey.ToString())),
                    q.Max,
                    q.Period.ToString()))
                .ToList();

            var tierQuotasForIdentity = identity.TierQuotas
                .Select(q => new QuotaDTO(
                    q.Id,
                    QuotaSource.Tier,
                    new MetricDTO(new Metric(q.MetricKey, q.MetricKey.ToString())),
                    q.Max,
                    q.Period.ToString()))
                .ToList();

            cachedQuotas = individualQuotasForIdentity.Concat(tierQuotasForIdentity).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Adjust the timespan according to your needs
            _cache.Set(cacheKey, cachedQuotas, cacheEntryOptions);
        }

        var totalRecords = cachedQuotas.Count;
        var pagedQuotas = PaginationHelper.ApplyPagedResponse(cachedQuotas, request.PaginationFilter);

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
