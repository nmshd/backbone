using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(ITokensRepository tokensRepository, ILogger<Handler> logger)
    {
        _tokensRepository = tokensRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteTokens(cancellationToken);
    }

    private async Task DeleteTokens(CancellationToken cancellationToken)
    {
        var numberOfDeletedItems = await _tokensRepository.Delete(Token.CanBeCleanedUp, cancellationToken);

        _logger.DataDeleted(numberOfDeletedItems, "tokens");
    }
}
