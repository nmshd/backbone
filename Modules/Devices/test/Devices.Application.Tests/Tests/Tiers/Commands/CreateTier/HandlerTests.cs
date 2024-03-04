using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests.Extensions;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.CreateTier;

public class HandlerTests
{
    private readonly Handler _handler;
    private readonly ITiersRepository _tierRepository;

    public HandlerTests()
    {
        _tierRepository = A.Fake<ITiersRepository>();
        _handler = CreateHandler();
    }

    private Handler CreateHandler()
    {
        var eventBus = A.Fake<IEventBus>();
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(_tierRepository, logger, eventBus);
    }

    [Fact]
    public async void Creates_a_tier_when_properties_are_valid()
    {
        // Arrange
        var expectedTierName = TierName.Create("my-tier-name");
        var expectedTier = new Tier(expectedTierName.Value);
        A.CallTo(() => _tierRepository.AddAsync(expectedTier, CancellationToken.None)).Returns(Task.FromResult(expectedTier));

        // Act
        var result = await _handler.Handle(new CreateTierCommand(expectedTierName.Value), CancellationToken.None);

        // Assert
        result.Id.Should().NotBeNull();
        result.Name.Should().Be(expectedTierName.Value);
    }

    [Fact]
    public async void Fails_to_create_a_tier_when_tier_name_already_exists()
    {
        // Arrange
        A.CallTo(() => _tierRepository.ExistsWithName(A<TierName>._, CancellationToken.None)).Returns(Task.FromResult(true));

        // Act
        var acting = async () => await _handler.Handle(new CreateTierCommand("my-tier-name"), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.tierNameAlreadyExists");
    }
}
