using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessGracePeriodReminders;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.SendDeletionProcessGracePeriodReminders;

public class HandlerTests
{
    [Fact]
    public async Task No_identities_with_an_approved_deletion_process_exist()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity>());

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<object>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Sends_first_reminder()
    {
        // Arrange
        IdentityDeletionConfiguration.LengthOfGracePeriod = 30;
        IdentityDeletionConfiguration.GracePeriodNotification1.Time = 20;

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(approvalDate: DateTime.Parse("2000-01-01"));

        var utcNow = DateTime.Parse("2000-01-11");
        SystemTime.Set(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessGracePeriodReminderPushNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
            i.Address == identity.Address
            && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == utcNow
        ), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_second_reminder()
    {
        // Arrange
        IdentityDeletionConfiguration.LengthOfGracePeriod = 30;
        IdentityDeletionConfiguration.GracePeriodNotification2.Time = 10;

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(approvalDate: DateTime.Parse("2000-01-01"));
        identity.DeletionGracePeriodReminder1Sent();

        var utcNow = DateTime.Parse("2000-01-21");
        SystemTime.Set(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessGracePeriodReminderPushNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == utcNow
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_third_reminder()
    {
        // Arrange
        IdentityDeletionConfiguration.LengthOfGracePeriod = 30;
        IdentityDeletionConfiguration.GracePeriodNotification3.Time = 5;

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(approvalDate: DateTime.Parse("2000-01-01"));
        identity.DeletionGracePeriodReminder1Sent();
        identity.DeletionGracePeriodReminder2Sent();

        var utcNow = DateTime.Parse("2000-01-26");
        SystemTime.Set(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessGracePeriodReminderPushNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder3SentAt == utcNow
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_send_reminder_1_when_2_has_to_be_sent_as_well()
    {
        // Arrange
        IdentityDeletionConfiguration.LengthOfGracePeriod = 30;
        IdentityDeletionConfiguration.GracePeriodNotification1.Time = 20;
        IdentityDeletionConfiguration.GracePeriodNotification2.Time = 10;

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(approvalDate: DateTime.Parse("2000-01-01"));

        var utcNow = DateTime.Parse("2000-01-21");
        SystemTime.Set(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessGracePeriodReminderPushNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == utcNow
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_send_reminder_1_and_2_when_3_has_to_be_sent_as_well()
    {
        // Arrange
        IdentityDeletionConfiguration.LengthOfGracePeriod = 30;
        IdentityDeletionConfiguration.GracePeriodNotification1.Time = 20;
        IdentityDeletionConfiguration.GracePeriodNotification2.Time = 10;
        IdentityDeletionConfiguration.GracePeriodNotification3.Time = 5;

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(approvalDate: DateTime.Parse("2000-01-01"));

        var utcNow = DateTime.Parse("2000-01-26");
        SystemTime.Set(utcNow);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodRemindersCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessGracePeriodReminderPushNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder3SentAt == utcNow
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        return new Handler(identitiesRepository, pushNotificationSender, A.Fake<ILogger<Handler>>());
    }
}
