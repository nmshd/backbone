using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsSupport;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository);

        // Acting
        var result = await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address), CancellationToken.None);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        result.Status.Should().Be(DeletionProcessStatus.Cancelled);
        result.CancelledAt.Should().Be(utcNow);
    }

    [Fact]
    public async void Publishes_IdentityDeletionProcessStartedEvent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(DateTime.Parse("2000-01-01"));

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeIdentitiesRepository, mockEventBus);

        // Act
        var response = await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address), CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<IdentityDeletionProcessStartedIntegrationEvent>.That.Matches(e =>
                    e.Address == identity.Address &&
                    e.DeletionProcessId == response.Id))
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                address,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository);

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionAsSupportCommand(address), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, CancelDeletionAsSupportResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return CreateHandler(identitiesRepository, A.Fake<IEventBus>());
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IEventBus eventBus)
    {
        return new Handler(identitiesRepository, eventBus);
    }
}
