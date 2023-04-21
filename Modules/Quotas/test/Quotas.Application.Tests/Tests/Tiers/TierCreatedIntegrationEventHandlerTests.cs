using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
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
        var id = "1";
        var name = "my-tier-name";
        
        var mockTierRepository = new AddMockTiersRepository();
        var handler = CreateHandler(mockTierRepository);

        // Act
        await handler.Handle(new TierCreatedIntegrationEvent() { Id = id, Name = name });

        // Assert
        mockTierRepository.WasCalled.Should().BeTrue();
        mockTierRepository.WasCalledWith.Id.Should().Be("1");
        mockTierRepository.WasCalledWith.Name.Should().Be("my-tier-name");
        
    }

    private TierCreatedIntegrationEventHandler CreateHandler(AddMockTiersRepository tiers)
    {
        return new TierCreatedIntegrationEventHandler(tiers, _logger);
    }
}