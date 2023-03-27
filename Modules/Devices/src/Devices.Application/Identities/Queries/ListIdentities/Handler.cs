using AutoMapper;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : RequestHandlerBase<ListIdentitiesQuery, ListIdentitiesResponse>
{
    public Handler(IDevicesDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper)
    {}

    public override async Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var identities = _dbContext.Set<Identity>().ToList();
        var query = _dbContext
            .Set<Identity>();

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);


        var identitiesDTOList = dbPaginationResult.ItemsOnPage.Select(el =>
        {
            return new IdentityDTO(el.Address, el.ClientId, el.PublicKey, el.IdentityVersion, el.CreatedAt);
        }).ToList();

        return new ListIdentitiesResponse(identitiesDTOList, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
