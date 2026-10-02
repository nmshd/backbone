using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokens;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository, IUserContext userContext)
    {
        _tokensRepository = tokensRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _tokensRepository.ListTokensAllocatedOrCreatedByWithContent(request.Ids, _activeIdentity, request.PaginationFilter, cancellationToken, track: false);

        return new Response(dbPaginationResult, request.PaginationFilter);
    }
}
