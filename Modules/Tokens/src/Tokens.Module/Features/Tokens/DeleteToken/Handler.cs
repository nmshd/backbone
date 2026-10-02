using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

public class Handler : IRequestHandler<Command>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, ITokensRepository tokensRepository)
    {
        _userContext = userContext;
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.GetWithoutContent(TokenId.Parse(request.Id), cancellationToken) ?? throw new NotFoundException(nameof(Token));

        token.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _tokensRepository.DeleteToken(token, cancellationToken);
    }
}
