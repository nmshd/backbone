using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Application.Tests.Extensions;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.DeleteTier;
public class HandlerTests
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
    public async Task Cannot_Delete_Basic_Tier()
    {
        // Arrange
        var basicTier = new Tier(TierName.BASIC_DEFAULT_NAME);
        A.CallTo(() => _tiersRepository.FindById(basicTier.Id, A<CancellationToken>._)).Returns(Task.FromResult(basicTier));

        // Act
        var acting = async () => await _handler.Handle(new DeleteTierCommand(basicTier.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.basicTierCannotBeDeleted");
    }

    [Fact]
    public async Task Cannot_Delete_Tier_With_Associated_Identities()
    {
        // Arrange
        var someIdentity = TestDataGenerator.CreateIdentity();
        var tier = new Tier(TierName.Create("tier-name").Value) { IdentityAddresses = new List<IdentityAddress> { TestDataGenerator.CreateRandomIdentityAddress() } };

        A.CallTo(() => _tiersRepository.FindById(tier.Id, A<CancellationToken>._)).Returns(Task.FromResult(tier));

        // Act
        var acting = async () => await _handler.Handle(new DeleteTierCommand(tier.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.usedTierCannotBeDeleted");
    }

    [Fact]
    public async Task Publishes_TierDeletedIntegrationEvent_after_successful_deletion()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value) { IdentityAddresses = new List<IdentityAddress>() };

        A.CallTo(() => _tiersRepository.FindById(tier.Id, A<CancellationToken>._)).Returns(Task.FromResult(tier));

        // Act
        await _handler.Handle(new DeleteTierCommand(tier.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<TierDeletedIntegrationEvent>._)).MustHaveHappened();
    }

    private Handler CreateHandler()
    {
        return new Handler(_tiersRepository, _eventBus);
    }
}
