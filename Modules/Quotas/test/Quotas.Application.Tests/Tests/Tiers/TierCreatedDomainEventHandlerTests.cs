using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Tiers;

public class TierCreatedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_tier_after_consuming_domain_event()
    {
        // Arrange
        var id = TierId.Parse("TIRFxoL0U24aUqZDSAWc");
        const string name = "Basic";
        var mockTierRepository = new AddMockTiersRepository();
        var handler = CreateHandler(mockTierRepository);

        // Act
        await handler.Handle(new TierCreatedDomainEvent(id, name));

        // Assert
        mockTierRepository.WasCalled.Should().BeTrue();
        mockTierRepository.WasCalledWith!.Id.Should().Be(id);
        mockTierRepository.WasCalledWith.Name.Should().Be(name);
    }

    private static TierCreatedDomainEventHandler CreateHandler(AddMockTiersRepository tiers)
    {
        var logger = A.Fake<ILogger<TierCreatedDomainEventHandler>>();
        return new TierCreatedDomainEventHandler(tiers, logger);
    }
}
