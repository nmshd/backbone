using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
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

        var metric = new Metric();
        var max = 5;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metric, max, period);
        var tierQuotaDefinitions = new List<TierQuotaDefinition> { tierQuotaDefinition };
        var tier = new Tier(tierId, "some-tier-name", tierQuotaDefinitions);
        var tiers = new List<Tier> { tier };

        var mockIdentitiesRepository = new MockIdentitiesRepository();
        var mockTiersRepository = new FindMockTiersRepository(tiers);

        var handler = CreateHandler(mockIdentitiesRepository, mockTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        mockIdentitiesRepository.WasAddCalled.Should().BeTrue();
        mockIdentitiesRepository.WasAddCalledWith.Address.Should().Be(address);
        mockIdentitiesRepository.WasAddCalledWith.TierId.Should().Be(tierId);

        mockIdentitiesRepository.WasUpdateCalled.Should().BeTrue();
        mockIdentitiesRepository.WasUpdateCalledWith.Address.Should().Be(address);
        mockIdentitiesRepository.WasUpdateCalledWith.TierId.Should().Be(tierId);
    }

    private IdentityCreatedIntegrationEventHandler CreateHandler(MockIdentitiesRepository identities, FindMockTiersRepository tiers)
    {
        var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
        return new IdentityCreatedIntegrationEventHandler(identities, logger, tiers);
    }
}
