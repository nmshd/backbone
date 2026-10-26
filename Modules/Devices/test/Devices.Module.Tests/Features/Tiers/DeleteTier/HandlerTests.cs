using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using DeleteTierSlice = Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;

namespace Backbone.Modules.Devices.Module.Tests.Features.Tiers.DeleteTier;

public class HandlerTests : AbstractTestsBase
{
    private readonly ITiersRepository _tiersRepository;
    private readonly Handler _handler;

    public HandlerTests()
    {
        _tiersRepository = A.Fake<ITiersRepository>();
        _handler = CreateHandler();
    }

    [Fact]
    public async Task Queues_TierDeletedDomainEvent_before_removing_the_tier()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        A.CallTo(() => _tiersRepository.Get(tier.Id, A<CancellationToken>._)).Returns(Task.FromResult<Tier?>(tier));
        A.CallTo(() => _tiersRepository.GetNumberOfClientsWithDefaultTier(tier, A<CancellationToken>._)).Returns(Task.FromResult(0));
        A.CallTo(() => _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, A<CancellationToken>._)).Returns(Task.FromResult(0));

        tier.ClearDomainEvents();
        A.CallTo(() => _tiersRepository.Remove(tier)).Invokes(() => tier.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<TierDeletedDomainEvent>());

        // Act
        await _handler.Handle(new DeleteTierSlice.Command { TierId = tier.Id }, CancellationToken.None);

        // Assert
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
        var acting = async () => await _handler.Handle(new DeleteTierSlice.Command { TierId = tier.Id }, CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<DomainException>();
        A.CallTo(() => _tiersRepository.Remove(tier)).MustNotHaveHappened();
        tier.DomainEvents.ShouldNotContain(e => e is TierDeletedDomainEvent);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Tier_deletion_and_event_are_committed_or_rolled_back_together(bool commit)
    {
        using var connection = FakeDbContextFactory.CreateDbConnection();
        var (arrange, act, assertion) = FakeDbContextFactory.CreateDbContexts<DevicesDbContext>(connection);
        await using var arrangeContext = arrange;
        await using var actContext = act;
        await using var assertionContext = assertion;
        var cancellationToken = TestContext.Current.CancellationToken;
        var tier = new Tier(TierName.Create("deletion-outbox-test").Value);
        // This test only observes the deletion event.
        tier.ClearDomainEvents();
        arrangeContext.Add(tier);
        await arrangeContext.SaveChangesAsync(cancellationToken);

        await using (var transaction = await actContext.Database.BeginTransactionAsync(cancellationToken))
        {
            var repository = new TiersRepository(actContext, null!);
            var persistedTier = (await repository.Get(tier.Id, cancellationToken))!;
            persistedTier.Delete();
            await repository.Remove(persistedTier);
            if (commit)
                await transaction.CommitAsync(cancellationToken);
            else
                await transaction.RollbackAsync(cancellationToken);
        }

        // A fresh context observes durable state without any broker being registered.
        (await assertionContext.Set<Tier>().AnyAsync(t => t.Id == tier.Id, cancellationToken)).ShouldBe(!commit);
        var messages = await assertionContext.Set<OutboxMessage>().ToListAsync(cancellationToken);
        if (commit)
        {
            var message = messages.ShouldHaveSingleItem();
            message.PublishedAt.ShouldBeNull();
            JsonSerializer.Deserialize<TierDeletedDomainEvent>(message.Payload)!.Id.ShouldBe(tier.Id.Value);
        }
        else
        {
            messages.ShouldBeEmpty();
        }
    }

    private Handler CreateHandler()
    {
        return new Handler(_tiersRepository);
    }
}
