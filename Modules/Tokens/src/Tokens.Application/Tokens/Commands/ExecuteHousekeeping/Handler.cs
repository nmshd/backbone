using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteTokens(cancellationToken);
    }

    private async Task DeleteTokens(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("tokens", ct => _tokensRepository.Delete(Token.CanBeCleanedUp, ct), cancellationToken);
    }
}
