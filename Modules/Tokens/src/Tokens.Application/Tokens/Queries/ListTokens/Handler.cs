using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class Handler : IRequestHandler<ListTokensQuery, ListTokensResponse>
{
    private readonly ITokensRepository _tokenRepository;
    private readonly IMapper _mapper;
    private readonly IdentityAddress _activeIdentity;

    public Handler(ITokensRepository tokenRepository, IUserContext userContext, IMapper mapper)
    {
        _tokenRepository = tokenRepository;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListTokensResponse> Handle(ListTokensQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = request.Ids.Any()
            ? await _tokenRepository.FindAllWithIds(request.Ids, request.PaginationFilter)
            : await _tokenRepository.FindAllOfOwner(_activeIdentity, request.PaginationFilter);

        return new ListTokensResponse(_mapper.Map<IEnumerable<TokenDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
