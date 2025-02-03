using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;

public class Handler : IRequestHandler<DeleteTokenCommand>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, ITokensRepository tokensRepository)
    {
        _userContext = userContext;
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(DeleteTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(TokenId.Parse(request.Id), cancellationToken) ?? throw new NotFoundException(nameof(Token));

        token.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _tokensRepository.DeleteToken(token, cancellationToken);
    }
}
