using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;

public class IdentityCreatedIntegrationEventHandlerTests
{
    [Fact]
    public async void Successfully_creates_identity_after_consuming_integration_event()
    {
        // Arrange
        var address = "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm";
        var tierId = "TIRFxoL0U24aUqZDSAWc";
        var tier = new Tier(tierId, "some-tier-name");
        var tiers = new List<Tier> { tier };

        var mockIdentitiesRepository = new AddMockIdentitiesRepository();
        var mockTiersRepository = new FindMockTiersRepository(tiers);

        var handler = CreateHandler(mockIdentitiesRepository, mockTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        mockIdentitiesRepository.WasCalled.Should().BeTrue();
        mockIdentitiesRepository.WasCalledWith.Address.Should().Be(address);
        mockIdentitiesRepository.WasCalledWith.TierId.Should().Be(tierId);
    }

    private IdentityCreatedIntegrationEventHandler CreateHandler(AddMockIdentitiesRepository identities, FindMockTiersRepository tiers)
    {
        var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
        return new IdentityCreatedIntegrationEventHandler(identities, logger, tiers);
    }
}
