using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ExecuteHousekeeping;

public class Handler : IRequestHandler<Command>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await DeleteTokens(cancellationToken);
    }

    private async Task DeleteTokens(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("tokens", ct => _tokensRepository.Delete(Token.CanBeCleanedUp, ct), cancellationToken);
    }
}
