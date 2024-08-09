using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Handler : IRequestHandler<CreateTokenCommand, CreateTokenResponse>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, ITokensRepository tokensRepository)
    {
        _userContext = userContext;
        _tokensRepository = tokensRepository;
    }

    public async Task<CreateTokenResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var newTokenEntity = new Token(_userContext.GetAddress(), _userContext.GetDeviceId(), request.Content, request.ExpiresAt);

        await _tokensRepository.Add(newTokenEntity);

        return new CreateTokenResponse(newTokenEntity);
    }
}
