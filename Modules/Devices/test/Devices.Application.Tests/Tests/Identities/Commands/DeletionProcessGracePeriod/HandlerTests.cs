using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.DeletionProcessGracePeriod;
public class HandlerTests
{
    [Fact]
    public async Task No_identities_with_an_approved_deletion_process_exist()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity>());

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<object>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Sends_First_Reminder()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        var reminderSentDate = identity.DeletionGracePeriodEndsAt!.Value.AddDays(-20);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>.That.Matches(i => i.StringValue.Length == identity.Address.StringValue.Length), A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
            i.Address == identity.Address
            && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == reminderSentDate
        ), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_Second_Reminder()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        identity.DeletionGracePeriodReminder1Sent();

        var reminderSentDate = identity.DeletionGracePeriodEndsAt!.Value.AddDays(-10);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>.That.Matches(i => i == identity.Address), A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == reminderSentDate
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_Third_Reminder()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        identity.DeletionGracePeriodReminder1Sent();
        identity.DeletionGracePeriodReminder2Sent();

        var reminderSentDate = identity.DeletionGracePeriodEndsAt!.Value.AddDays(-5);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>.That.Matches(i => i == identity.Address), A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder3SentAt == reminderSentDate
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Only_Sends_Second_Reminder_When_Date_Condition_Is_Met_And_First_Reminder_Has_Not_Been_Previously_Sent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        var reminderSentDate = identity.DeletionGracePeriodEndsAt!.Value.AddDays(-10);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>.That.Matches(i => i == identity.Address), A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == reminderSentDate
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Only_Sends_Third_Reminder_When_Date_Condition_Is_Met_And_Others_Have_Not_Been_Previously_Sent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        var reminderSentDate = identity.DeletionGracePeriodEndsAt!.Value.AddDays(-5);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithApprovedDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { identity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new SendDeletionProcessGracePeriodReminderCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>.That.Matches(i => i == identity.Address), A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder1SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder2SentAt == null
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodReminder3SentAt == reminderSentDate
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
