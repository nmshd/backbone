using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessApprovalReminder;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Handler = Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessApprovalReminder.Handler;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.SendDeletionProcessApprovalReminder;

public class HandlerTests
{
    [Fact]
    public async void Sends_first_reminder_when_not_previously_sent()
    {
        // Arrange
        SystemTime.Set(DateTime.UtcNow);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsSupport();
        var identitiesList = new List<Identity> { identity };

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._))
            .Returns(identitiesList);

        var handler = CreateHandler(fakeIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappenedOnceExactly();

        var deletionProcess = identity.DeletionProcesses.First();
        deletionProcess.ApprovalReminder1SentAt?.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Should().HaveCount(2);

        var reminder1AuditLogEntry = deletionProcess.AuditLog.Second();
        reminder1AuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminder1AuditLogEntry.Message.Should().Be("Approval reminder 1 was sent.");
    }

    [Fact]
    public async void Sends_second_reminder_when_first_was_previously_sent()
    {
        // Arrange
        SystemTime.Set(DateTime.UtcNow);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsSupport();
        var identitiesList = new List<Identity> { identity };

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._))
            .Returns(identitiesList);

        var handler = CreateHandler(fakeIdentitiesRepository, mockPushNotificationSender);
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappenedTwiceExactly();

        var deletionProcess = identity.DeletionProcesses.First();
        deletionProcess.ApprovalReminder2SentAt?.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Should().HaveCount(3);

        var reminder2AuditLogEntry = deletionProcess.AuditLog.ElementAt(2);
        reminder2AuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminder2AuditLogEntry.Message.Should().Be("Approval reminder 2 was sent.");
    }

    [Fact]
    public async void Sends_third_reminder_when_first_and_second_were_previously_sent()
    {
        // Arrange
        SystemTime.Set(DateTime.UtcNow);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsSupport();
        var identitiesList = new List<Identity> { identity };

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._))
            .Returns(identitiesList);

        var handler = CreateHandler(fakeIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappenedOnceExactly();

        var deletionProcess = identity.DeletionProcesses.First();
        deletionProcess.ApprovalReminder1SentAt?.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Should().HaveCount(2);
        deletionProcess.AuditLog.Second().CreatedAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Second().Message.Should().Be("Approval reminder 1 was sent.");
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender = null)
    {
        pushNotificationSender ??= A.Fake<IPushNotificationSender>();
        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
