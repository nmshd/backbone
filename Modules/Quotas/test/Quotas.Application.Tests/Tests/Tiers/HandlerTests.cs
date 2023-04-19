using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;
public class HandlerTests
{
    private readonly ILogger<TierCreatedIntegrationEventHandler> _tiersLogger;

    public HandlerTests() { }

    [Fact]
    public async void Returns_add_when_tiers_created()
    {
        // Arrange
        var id = "1";
        var name = "my-tier-name";
        var tiers = new Tier(id, name);

        var handler = CreateHandler(new AddTiersRepository(tiers));

        // Act
        var result = handler.Handle(new TierCreatedIntegrationEvent { Name = name, Id = id });

        // Assert
        result.Should().NotBeNull();
    }

    private TierCreatedIntegrationEventHandler CreateHandler(AddTiersRepository tiers)
    {
        return new TierCreatedIntegrationEventHandler(tiers, _tiersLogger);
    }
}