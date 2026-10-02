using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Module.Tests.TestDoubles;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using TierCreatedSlice = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierCreated;

namespace Backbone.Modules.Quotas.Module.Tests.Features.DomainEvents.TierCreated;

public class HandlerTests : AbstractTestsBase
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
        await handler.Handle(new TierCreatedDomainEvent { Id = id, Name = name });

        // Assert
        mockTierRepository.WasCalled.ShouldBeTrue();
        mockTierRepository.WasCalledWith!.Id.ShouldBe(id);
        mockTierRepository.WasCalledWith.Name.ShouldBe(name);
    }

    private static TierCreatedSlice.Handler CreateHandler(AddMockTiersRepository tiers)
    {
        var logger = A.Fake<ILogger<TierCreatedSlice.Handler>>();
        return new TierCreatedSlice.Handler(tiers, logger);
    }
}
