using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;
public class TierCreatedIntegrationEventHandlerTests
{
    [Fact]
    public async void Creates_tier_after_consuming_integration_event()
    {
        // Arrange
        var id = new TierId("TIRFxoL0U24aUqZDSAWc");
        var name = "Basic";
        var mockTierRepository = new AddMockTiersRepository();
        var handler = CreateHandler(mockTierRepository);

        // Act
        await handler.Handle(new TierCreatedIntegrationEvent(id, name));

        // Assert
        mockTierRepository.WasCalled.Should().BeTrue();
        mockTierRepository.WasCalledWith!.Id.Should().Be(id);
        mockTierRepository.WasCalledWith.Name.Should().Be(name);
    }

    private TierCreatedIntegrationEventHandler CreateHandler(AddMockTiersRepository tiers)
    {
        var logger = A.Fake<ILogger<TierCreatedIntegrationEventHandler>>();
        return new TierCreatedIntegrationEventHandler(tiers, logger);
    }
}
