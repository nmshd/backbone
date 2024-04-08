using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class Handler : IRequestHandler<ListTokensQuery, ListTokensResponse>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IMapper _mapper;
    private readonly IdentityAddress _activeIdentity;

    public Handler(ITokensRepository tokensRepository, IUserContext userContext, IMapper mapper)
    {
        _tokensRepository = tokensRepository;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListTokensResponse> Handle(ListTokensQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = request.Ids.Any()
            ? await _tokensRepository.FindAllWithIds(request.Ids, request.PaginationFilter, cancellationToken)
            : await _tokensRepository.FindAllOfOwner(_activeIdentity, request.PaginationFilter, cancellationToken);

        return new ListTokensResponse(_mapper.Map<IEnumerable<TokenDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
