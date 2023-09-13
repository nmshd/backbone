using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;
public class HandlerTests
{
    [Fact]
    public async void Command_runs_successfully_updates_identity()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Old tier").Value);
        var newTier = new Tier(TierName.Create("New Tier").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindById(oldTier.Id, A<CancellationToken>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.FindById(newTier.Id, A<CancellationToken>._)).Returns(newTier);

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.TierId.Should().Be(newTier.Id);
    }

    [Fact]
    public async void Command_runs_successfully_publishes_IntegrationEvent()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Old tier").Value);
        var newTier = new Tier(TierName.Create("New Tier").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);
        A.CallTo(() => tiersRepository.FindById(oldTier.Id, A<CancellationToken>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.FindById(newTier.Id, A<CancellationToken>._)).Returns(newTier);

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => eventBus.Publish(A<TierOfIdentityChangedIntegrationEvent>._)).MustHaveHappened();
    }

    [Fact]
    public async void Command_fails_when_identity_is_missing()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => tiersRepository.FindById(oldTier.Id, A<CancellationToken>._)).Returns(oldTier);
        A.CallTo(() => tiersRepository.FindById(newTier.Id, A<CancellationToken>._)).Returns(newTier);

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
    public async void Command_fails_when_tier_is_missing()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();
        var eventBus = A.Fake<IEventBus>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(TestDataGenerator.CreateRandomDeviceId(), TestDataGenerator.CreateRandomIdentityAddress(), new byte[] { 1, 1, 1, 1, 1 }, oldTier.Id, 1);

        A.CallTo(() => identitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._)).Returns(identity);

        var handler = CreateHandler(identitiesRepository, tiersRepository, eventBus);
        var request = MakeRequest(newTier, identity);

        // Act
        Func<Task> acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Tier");
        exception.Code.Should().Be("error.platform.recordNotFound");
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
