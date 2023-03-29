using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class Handler : IRequestHandler<CreateTierCommand, CreateTierResponse>
{
    private readonly IDevicesDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(IDevicesDbContext dbContext, ILogger<Handler> logger, IEventBus eventBus)
    {
        _dbContext = dbContext;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<CreateTierResponse> Handle(CreateTierCommand request, CancellationToken cancellationToken)
    {
        var tierName = TierName.Create(request.Name);
        var tier = new Tier(tierName.Value);

        await _dbContext.Set<Tier>().AddAsync(tier, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogTrace($"Successfully created tier. Tier ID: {tier.Id.Value}, Tier Name: {tier.Name.Value}");

        _eventBus.Publish(new TierCreatedIntegrationEvent(tier));

        _logger.LogTrace($"Successfully published TierCreatedIntegrationEvent. Tier ID: {tier.Id.Value}, Tier Name: {tier.Name.Value}");

        return new CreateTierResponse
        {
            Id = tier.Id,
            Name = tier.Name
        };
    }
}
