using AutoMapper;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Extensions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : IRequestHandler<ListIdentitiesQuery, ListIdentitiesResponse>
{
    private readonly IDevicesDbContext _dbContext;

    public Handler(IDevicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
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
