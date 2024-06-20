﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsOwner;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var activeDevice = activeIdentity.Devices[0];
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(activeIdentity.Address, CancellationToken.None, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext, mockPushNotificationSender);

        // Act
        var response = await handler.Handle(new CancelDeletionProcessAsOwnerCommand(deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == activeIdentity.Address
                && i.Status == IdentityStatus.Active
                && i.DeletionProcesses[0].Status == DeletionProcessStatus.Cancelled
                && i.DeletionProcesses[0].Id == deletionProcess.Id), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        response.Status.Should().Be(DeletionProcessStatus.Cancelled);

        A.CallTo(() => mockPushNotificationSender.SendNotification(activeIdentity.Address, A<DeletionProcessCancelledByOwnerNotification>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var handler = CreateHandler();

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionProcessAsOwnerCommand(address), CancellationToken.None);

        // Assert
        acting.Should().ThrowAsync<NotFoundException>();
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IUserContext? userContext = null, IPushNotificationSender? pushNotificationSender = null)
    {
        userContext ??= A.Dummy<IUserContext>();
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();

        return new Handler(identitiesRepository, userContext, pushNotificationSender);
    }
}
