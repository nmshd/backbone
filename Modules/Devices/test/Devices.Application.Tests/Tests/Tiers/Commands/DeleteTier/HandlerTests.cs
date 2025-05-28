using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.DeleteTier;

public class HandlerTests : AbstractTestsBase
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IEventBus _eventBus;
    private readonly Handler _handler;

    public HandlerTests()
    {
        _tiersRepository = A.Fake<ITiersRepository>();
        _eventBus = A.Fake<IEventBus>();
        _handler = CreateHandler();
    }

    [Fact]
    public async Task Deletes_the_tier_from_the_database_and_publishes_TierDeletedDomainEvent()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        A.CallTo(() => _tiersRepository.Get(tier.Id, A<CancellationToken>._)).Returns(Task.FromResult<Tier?>(tier));
        A.CallTo(() => _tiersRepository.GetNumberOfClientsWithDefaultTier(tier, A<CancellationToken>._)).Returns(Task.FromResult(0));
        A.CallTo(() => _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, A<CancellationToken>._)).Returns(Task.FromResult(0));

        // Act
        await _handler.Handle(new DeleteTierCommand(tier.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<TierDeletedDomainEvent>._)).MustHaveHappened();
        A.CallTo(() => _tiersRepository.Remove(tier)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Cannot_delete_tier_if_CanBeDeleted_returns_an_error()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        A.CallTo(() => _tiersRepository.Get(tier.Id, A<CancellationToken>._)).Returns(Task.FromResult<Tier?>(tier));
        A.CallTo(() => _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, A<CancellationToken>._)).Returns(1);
        A.CallTo(() => _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, A<CancellationToken>._)).Returns(1);

        // Act
        var acting = async () => await _handler.Handle(new DeleteTierCommand(tier.Id), CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<DomainException>();
        A.CallTo(() => _tiersRepository.Remove(tier)).MustNotHaveHappened();
    }

    private Handler CreateHandler()
    {
        return new Handler(_tiersRepository, _eventBus);
    }
}
