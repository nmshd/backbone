using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using Xunit;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Microsoft.AspNetCore.Http.HttpResults;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentAssertions;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcess;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockEventBus = A.Fake<IEventBus>();

        var identityAddress = IdentityAddress.Create(new byte[] { }, "id1");

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns(new Identity("", identityAddress, new byte[] { }, TierId.Generate(), 1));

        var handler = new Handler(mockIdentitiesRepository, mockEventBus);

        // Act
        await handler.Handle(new StartDeletionProcessCommand(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.DeletionProcesses.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(
                A<IdentityDeletionProcessStartedIntegrationEvent>.That.Matches(e => e.IdentityAddress == identityAddress)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Throws_NotFoundException_when_identity_is_not_found()
    {
        // Arrange
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeEventBus = A.Dummy<IEventBus>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns((Identity)null);

        var handler = new Handler(fakeIdentitiesRepository, fakeEventBus);

        // Act
        Func<Task> acting = async () => await handler.Handle(new StartDeletionProcessCommand(IdentityAddress.Create(new byte[] { }, "id1")), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException>().Which.Message.Should().Contain("Identity");
    }

    // TODO: refactoring
}
