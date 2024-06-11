using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.ApproveDeletionProcess;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.Parse("2000-01-10"));
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var device = identity.Devices[0];

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(device.Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var mockPushNotificationSender = A.Dummy<IPushNotificationSender>();

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext, mockPushNotificationSender);

        // Act
        var response = await handler.Handle(new ApproveDeletionProcessCommand(deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.Status == IdentityStatus.ToBeDeleted
                && i.TierId == Tier.QUEUED_FOR_DELETION.Id
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.ApprovedAt == DateTime.Parse("2000-01-01")
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodEndsAt == DateTime.Parse("2000-01-15")
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.ApprovedByDevice == device.Id
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessApprovedNotification>.That.Matches(n => n.DaysUntilDeletion == IdentityDeletionConfiguration.LengthOfGracePeriod), A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();

        response.Id.Should().Be(deletionProcess.Id);
        response.ApprovedAt.Should().Be(utcNow);
        response.ApprovedByDevice.Should().Be(device.Id);
        response.Status.Should().Be(DeletionProcessStatus.Approved);
    }

    [Fact]
    public void Throws_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(address, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(address);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new ApproveDeletionProcessCommand("some-deletion-process-id"), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, ApproveDeletionProcessResponse>().Which.Message.Should().Contain("Identity");
    }

    [Fact]
    public async Task Publishes_domain_events()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(new DateTime());
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var device = identity.Devices[0];
        var oldTierId = identity.TierId;

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(device.Id);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext, mockEventBus);

        // Act
        await handler.Handle(new ApproveDeletionProcessCommand(deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<TierOfIdentityChangedDomainEvent>.That.Matches(i =>
                i.IdentityAddress == identity.Address &&
                i.OldTierId == oldTierId &&
                i.NewTierId == Tier.QUEUED_FOR_DELETION.Id))
            ).MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(A<IdentityToBeDeletedDomainEvent>.That.Matches(i =>
                i.IdentityAddress == identity.Address))
            ).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IEventBus? eventBus = null, IPushNotificationSender? pushNotificationSender = null)
    {
        eventBus ??= A.Dummy<IEventBus>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        return new Handler(identitiesRepository, userContext, eventBus, pushNotificationSender);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IPushNotificationSender pushNotificationSender)
    {
        return CreateHandler(identitiesRepository, userContext, null, pushNotificationSender);
    }
}
