using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        var forIdentity = request.ForIdentity == null ? null : IdentityAddress.Parse(request.ForIdentity);
        var newToken = new Token(_userContext.GetAddressOrNull(), _userContext.GetDeviceIdOrNull(), request.Content, request.ExpiresAt, forIdentity, request.Password);

        await _tokensRepository.Add(newToken);

        return new CreateTokenResponse(newToken);
    }
}
