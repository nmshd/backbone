using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;
public class TierCreatedIntegrationEventHandlerTests
{
    private readonly ILogger<TierCreatedIntegrationEventHandler> _logger;

    public TierCreatedIntegrationEventHandlerTests() 
    {
        _logger = A.Fake<ILogger<TierCreatedIntegrationEventHandler>>();
    }

    [Fact]
    public async void Successfully_creates_tier_after_consuming_integration_event()
    {
        // Arrange
        var mockTierRepository = new AddMockTiersRepository();
        var handler = CreateHandler(mockTierRepository);

        // Act
        await handler.Handle(new TierCreatedIntegrationEvent() { });

        // Assert
        mockTierRepository.WasCalled.Should().BeTrue();
        mockTierRepository.WasCalledWith.Id.Should().Be(null);
        mockTierRepository.WasCalledWith.Name.Should().Be(null);
        
    }

    private TierCreatedIntegrationEventHandler CreateHandler(AddMockTiersRepository tiers)
    {
        return new TierCreatedIntegrationEventHandler(tiers, _logger);
    }
}