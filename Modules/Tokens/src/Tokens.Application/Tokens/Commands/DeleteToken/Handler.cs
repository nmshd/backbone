using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;

public class Handler : IRequestHandler<DeleteTokenCommand>
{
    private readonly ILogger<Handler> _logger;
    private readonly ITokensRepository _tokensRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IUserContext userContext, ILogger<Handler> logger, ITokensRepository tokensRepository) : base()
    {
        _logger = logger;
        _tokensRepository = tokensRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task Handle(DeleteTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(request.Id);

        if (token.CreatedBy != _activeIdentity)
            throw new ActionForbiddenException();

        await _tokensRepository.Remove(token);

        _logger.LogTrace($"Successfully deleted token with id {token.Id}.");
    }
}
