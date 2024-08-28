using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class Handler : IRequestHandler<ListTokensQuery, ListTokensResponse>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(ITokensRepository tokensRepository, IUserContext userContext)
    {
        _tokensRepository = tokensRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListTokensResponse> Handle(ListTokensQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = request.Ids.Any()
            ? await _tokensRepository.FindAllWithIds(request.Ids.Select(TokenId.Parse), request.PaginationFilter, cancellationToken)
            : await _tokensRepository.FindAllOfOwner(_activeIdentity, request.PaginationFilter, cancellationToken);

        return new ListTokensResponse(dbPaginationResult, request.PaginationFilter);
    }
}
