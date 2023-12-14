using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessApprovalReminder;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
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
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder1.Time);
        SystemTime.Set(reminderSentDate);

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappened();

        A.CallTo(() => mockIdentitiesRepository.Update(identity, A<CancellationToken>._))
            .MustHaveHappened();

        deletionProcess.ApprovalReminder1SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Any(a => a.Message == "First approval reminder was sent." && a.CreatedAt == reminderSentDate).Should().BeTrue();
    }

    [Fact]
    public async void Does_not_send_first_reminder_when_previously_sent()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder1.Time);
        SystemTime.Set(reminderSentDate);
        identity.DeletionProcessApprovalReminder1Sent();

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        var futureDate = DateTime.Parse("2002-02-02");
        SystemTime.Set(futureDate);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        deletionProcess.ApprovalReminder1SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Count(a => a.Message == "First approval reminder was sent.").Should().Be(1);
    }

    [Fact]
    public async void Sends_second_reminder_when_not_previously_sent()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();
        identity.DeletionProcessApprovalReminder1Sent();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder2.Time);
        SystemTime.Set(reminderSentDate);

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappened();

        A.CallTo(() => mockIdentitiesRepository.Update(identity, A<CancellationToken>._))
            .MustHaveHappened();

        deletionProcess.ApprovalReminder2SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Any(a => a.Message == "Second approval reminder was sent." && a.CreatedAt == reminderSentDate).Should().BeTrue();
    }

    [Fact]
    public async void Does_not_send_second_reminder_when_previously_sent()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder2.Time);
        SystemTime.Set(reminderSentDate);

        identity.DeletionProcessApprovalReminder1Sent();
        identity.DeletionProcessApprovalReminder2Sent();

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        var futureDate = DateTime.Parse("2002-02-02");
        SystemTime.Set(futureDate);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        deletionProcess.ApprovalReminder2SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Count(a => a.Message == "Second approval reminder was sent.").Should().Be(1);
    }

    [Fact]
    public async void Sends_third_reminder_when_not_previously_sent()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();
        identity.DeletionProcessApprovalReminder1Sent();
        identity.DeletionProcessApprovalReminder2Sent();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder3.Time);
        SystemTime.Set(reminderSentDate);

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessWaitingForApprovalReminderPushNotification>._, CancellationToken.None)
        ).MustHaveHappened();

        A.CallTo(() => mockIdentitiesRepository.Update(identity, A<CancellationToken>._))
            .MustHaveHappened();

        deletionProcess.ApprovalReminder3SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Any(a => a.Message == "Third approval reminder was sent." && a.CreatedAt == reminderSentDate).Should().BeTrue();
    }

    [Fact]
    public async void Does_not_send_third_reminder_when_previously_sent()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
        var reminderSentDate = endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder3.Time);
        SystemTime.Set(reminderSentDate);
        identity.DeletionProcessApprovalReminder1Sent();
        identity.DeletionProcessApprovalReminder2Sent();
        identity.DeletionProcessApprovalReminder3Sent();

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessWaitingForApproval(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        var futureDate = DateTime.Parse("2002-02-02");
        SystemTime.Set(futureDate);

        // Act
        await handler.Handle(new SendDeletionProcessApprovalReminderCommand(), CancellationToken.None);

        // Assert
        deletionProcess.ApprovalReminder3SentAt?.Should().Be(reminderSentDate);
        deletionProcess.AuditLog.Count(a => a.Message == "Second approval reminder was sent.").Should().Be(1);
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender = null)
    {
        pushNotificationSender ??= A.Fake<IPushNotificationSender>();
        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
