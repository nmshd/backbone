using System.Linq.Expressions;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.AdminApi.Queries.GetAllTokens;

public class Handler(ITokensRepository tokensRepository) : IRequestHandler<GetTokensQuery, GetTokensResponse>
{
    public async Task<GetTokensResponse> Handle(GetTokensQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await tokensRepository.FindAllTokens(request.PaginationFilter, GetFilter(request.CreatedBy), cancellationToken);
        var pagedResult = new GetTokensResponse(dbPaginationResult, request.PaginationFilter);

        return pagedResult;
    }

    private static Expression<Func<Token, bool>> GetFilter(string createdBy)
    {
        return token => token.CreatedBy == createdBy;
    }
}
