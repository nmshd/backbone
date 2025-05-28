using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokensByIdentity;

public class Handler(ITokensRepository tokensRepository) : IRequestHandler<ListTokensByIdentityQuery, ListTokensResponse>
{
    public async Task<ListTokensResponse> Handle(ListTokensByIdentityQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await tokensRepository.List(request.PaginationFilter, Token.WasCreatedBy(request.CreatedBy), cancellationToken);
        var pagedResult = new ListTokensResponse(dbPaginationResult, request.PaginationFilter);

        return pagedResult;
    }
}
