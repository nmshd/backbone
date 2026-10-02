using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokensByIdentity;

public class Handler(ITokensRepository tokensRepository) : IRequestHandler<Query, Response>
{
    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await tokensRepository.ListWithoutContent(request.PaginationFilter, Token.WasCreatedBy(request.CreatedBy), cancellationToken);
        var pagedResult = new Response(dbPaginationResult, request.PaginationFilter);

        return pagedResult;
    }
}
