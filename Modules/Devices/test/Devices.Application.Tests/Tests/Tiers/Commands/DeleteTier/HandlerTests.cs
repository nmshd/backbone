using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Backbone.Modules.Devices.Application.Tests.Extensions;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.DeleteTier;
public class HandlerTests
{
    private readonly ITiersRepository _tierRepository;
    private readonly IEventBus _eventBus;
    private readonly Handler _handler;

    public HandlerTests()
    {
        _tierRepository = A.Fake<ITiersRepository>();
        _eventBus = A.Fake<IEventBus>();
        _handler = CreateHandler();
    }

    private Handler CreateHandler()
    {
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(_tierRepository, logger, _eventBus);
    }

    [Fact]
    public async Task Cannot_Delete_Basic_Tier()
    {
        // Arrange
        var basicTierName = TierName.Create("basic-tier").Value;
        var basicTier = new Tier(basicTierName);
        A.CallTo(() => _tierRepository.GetBasicTierAsync(A<CancellationToken>._)).Returns(Task.FromResult(basicTier));
        A.CallTo(() => _tierRepository.FindById(basicTier.Id, A<CancellationToken>._)).Returns(Task.FromResult(basicTier));

        // Act
        var acting = async () => await _handler.Handle(new DeleteTierCommand(basicTier.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.basicTierCannotBeDeleted");
    }

    [Fact]
    public async Task Cannot_Delete_Tier_With_Associated_Identities()
    {
        // Arrange
        var basicTier = new Tier(TierName.Create("basic-tier").Value);
        var someIdentity = new Identity(
            TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            TestDataGenerator.CreateRandomTierId(),
            1);
        var anotherTier = new Tier(TierName.Create("random-tier").Value) { Identities = new List<Identity>() { someIdentity } };

        A.CallTo(() => _tierRepository.GetBasicTierAsync(A<CancellationToken>._)).Returns(Task.FromResult(basicTier));
        A.CallTo(() => _tierRepository.FindById(anotherTier.Id, A<CancellationToken>._)).Returns(Task.FromResult(anotherTier));

        // Act
        var acting = async () => await _handler.Handle(new DeleteTierCommand(anotherTier.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.usedTierCannotBeDeleted");
    }

    [Fact]
    public async Task Delete_Non_Basic_Tier_Without_Identities_Publishes_IntegrationEvent()
    {
        // Arrange
        var basicTier = new Tier(TierName.Create("basic-tier").Value);
        var anotherTier = new Tier(TierName.Create("random-tier").Value) { Identities = new List<Identity>() };

        A.CallTo(() => _tierRepository.GetBasicTierAsync(A<CancellationToken>._)).Returns(Task.FromResult(basicTier));
        A.CallTo(() => _tierRepository.FindById(anotherTier.Id, A<CancellationToken>._)).Returns(Task.FromResult(anotherTier));

        // Act
        await _handler.Handle(new DeleteTierCommand(anotherTier.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<TierDeletedIntegrationEvent>._)).MustHaveHappened();
    }
}
