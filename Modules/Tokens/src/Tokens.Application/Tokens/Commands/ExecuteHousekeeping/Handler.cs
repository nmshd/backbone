using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly HousekeepingTelemetry _telemetry;
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository, HousekeepingTelemetry telemetry)
    {
        _tokensRepository = tokensRepository;
        _telemetry = telemetry;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteTokens(cancellationToken);
    }

    private async Task DeleteTokens(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("tokens", ct => _tokensRepository.Delete(Token.CanBeCleanedUp, ct), cancellationToken);
    }
}
