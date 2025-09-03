using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.UpdateTokenContent;

public class Handler : IRequestHandler<UpdateTokenContentCommand, UpdateTokenContentResponse>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(ITokensRepository tokensRepository, IUserContext userContext)
    {
        _tokensRepository = tokensRepository;
        _userContext = userContext;
    }

    public async Task<UpdateTokenContentResponse> Handle(UpdateTokenContentCommand request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.GetWithContent(TokenId.Parse(request.TokenId), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Token));

        var result = token.UpdateContent(request.NewContent, _userContext.GetAddress(), _userContext.GetDeviceId(), request.Password);

        switch (result)
        {
            case UpdateTokenContentResult.WrongPassword:
                await _tokensRepository.Update(token, cancellationToken);
                throw new NotFoundException(nameof(Token));

            case UpdateTokenContentResult.Locked:
                await _tokensRepository.Update(token, cancellationToken);
                throw new ApplicationException(ApplicationErrors.TokenIsLocked());

            case UpdateTokenContentResult.ForIdentityDoesNotMatch or UpdateTokenContentResult.Expired:
                throw new NotFoundException(nameof(Token));

            case UpdateTokenContentResult.Ok:
                return new UpdateTokenContentResponse(token);

            default:
                throw new Exception("Unexpected token access result.");
        }
    }
}
