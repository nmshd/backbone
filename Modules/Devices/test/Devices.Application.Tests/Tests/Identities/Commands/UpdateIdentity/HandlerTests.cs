using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;
public class HandlerTests
{
    [Fact]
    public async void Updates_the_identity_in_the_database()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Old tier").Value);
        var newTier = new Tier(TierName.Create("New Tier").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier>() { oldTier, newTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identity.TierId.Should().Be(newTier.Id);
        A.CallTo(() => identitiesRepository.Update(identity, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Publishes_TierOfIdentityChangedIntegrationEvent()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Old tier").Value);
        var newTier = new Tier(TierName.Create("New Tier").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier>() { oldTier, newTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => eventBus.Publish(A<TierOfIdentityChangedIntegrationEvent>._)).MustHaveHappened();
    }

    [Fact]
    public async void Fails_when_identity_does_not_exist()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => tiersRepository.FindByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier>() { oldTier, newTier });
        A.CallTo(() => identitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._)).Returns((Identity)null);

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        Func<Task> acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public async void Fails_when_tier_does_not_exist()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier>() { oldTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        Func<Task> acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Tier");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public async void Does_nothing_when_tiers_are_the_same()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = oldTier;

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier>() { oldTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        var acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<DomainException>();
        A.CallTo(() => identitiesRepository.Update(A<Identity>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => eventBus.Publish(A<TierOfIdentityChangedIntegrationEvent>._)).MustNotHaveHappened();
    }

    private static UpdateIdentityCommand MakeRequest(Tier newTier, Identity identity)
    {
        return new()
        {
            Address = identity.Address,
            TierId = newTier.Id
        };
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IEventBus eventBus)
    {
        return new Handler(identitiesRepository, tiersRepository, eventBus);
    }
}
