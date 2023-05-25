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
        var address = "some-dummy-address";
        var tierId = new TierId("TIRsomeTierId1111111");

        var tier = new Tier(tierId, "some-tier-name");

        var mockIdentitiesRepository = new MockIdentitiesRepository();
        var stubTiersRepository = new FindTiersStubRepository(tier);

        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        mockIdentitiesRepository.WasAddCalled.Should().BeTrue();
        mockIdentitiesRepository.WasAddCalledWith.Address.Should().Be(address);
        mockIdentitiesRepository.WasAddCalledWith.TierId.Should().Be(tierId);
    }

    [Fact]
    public async void Successfully_creates_identity_and_assigns_tier_quotas_after_consuming_integration_event()
    {
        // Arrange
        var address = "some-dummy-address";
        var tierId = new TierId("TIRsomeTierId1111111");

        var max = 5;
        var tier = new Tier(tierId, "some-tier-name");
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Month));
        tier.Quotas.Add(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Week));

        var mockIdentitiesRepository = new MockIdentitiesRepository();
        var stubTiersRepository = new FindTiersStubRepository(tier);

        var handler = CreateHandler(mockIdentitiesRepository, stubTiersRepository);

        // Act
        await handler.Handle(new IdentityCreatedIntegrationEvent(address, tierId));

        // Assert
        mockIdentitiesRepository.WasAddCalled.Should().BeTrue();
        mockIdentitiesRepository.WasAddCalledWith.Address.Should().Be(address);
        mockIdentitiesRepository.WasAddCalledWith.TierId.Should().Be(tierId);
        mockIdentitiesRepository.WasAddCalledWith.TierQuotas.Should().HaveCount(2);
    }

    private IdentityCreatedIntegrationEventHandler CreateHandler(MockIdentitiesRepository identities, FindTiersStubRepository tiers)
    {
        var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
        return new IdentityCreatedIntegrationEventHandler(identities, logger, tiers);
    }
}
