﻿using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class Handler : QueryHandlerBase<ListTokensQuery, ListTokensResponse>
{
    public Handler(ITokenRepository tokenRepository, IUserContext userContext, IMapper mapper) : base(tokenRepository, userContext, mapper) { }

    public override async Task<ListTokensResponse> Handle(ListTokensQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = request.Ids.Any()
            ? await _tokenRepository.FindAllWithIds(request.Ids, request.PaginationFilter)
            : await _tokenRepository.FindAllOfOwner(_activeIdentity, request.PaginationFilter);

        return new ListTokensResponse(_mapper.Map<IEnumerable<TokenDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
