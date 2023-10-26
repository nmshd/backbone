using System.Threading;
using AutoMapper;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Google.Api;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIndividualQuotas;

public class Handler : IRequestHandler<ListIndividualQuotasQuery, ListIndividualQuotasResponse>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IMapper mapper, IMemoryCache cache, IIdentitiesRepository identitiesRepository)
    {
        _mapper = mapper;
        _cache = cache;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<ListIndividualQuotasResponse> Handle(ListIndividualQuotasQuery request, CancellationToken cancellationToken)
    {
        const string cacheKeyPrefix = "IndividualQuotaCachePrefix_";
        const int totalRecords = 10; // todo: not sure how I could pass this through

        PagedResponse<IndividualQuotaDTO> pagedResponse;

        if (_cache.TryGetValue(cacheKeyPrefix + request.Address, out List<IndividualQuotaDTO> items))
        {
            pagedResponse = new PagedResponse<IndividualQuotaDTO>(items, request.PaginationFilter, totalRecords); // todo: get rid of redundancy
            return new ListIndividualQuotasResponse(pagedResponse, request.PaginationFilter, totalRecords);
        }
        
        var identity = await _identitiesRepository.FindByAddresses(new List<string> { request.Address }.AsReadOnly(), cancellationToken, true);
        items = _mapper.Map<List<IndividualQuotaDTO>>(identity.First().IndividualQuotas);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };
        
        _cache.Set(cacheKeyPrefix + request.Address, items, cacheOptions);

        pagedResponse = new PagedResponse<IndividualQuotaDTO>(items, request.PaginationFilter, totalRecords);
    
        return new ListIndividualQuotasResponse(pagedResponse, request.PaginationFilter, totalRecords);
    }
}
