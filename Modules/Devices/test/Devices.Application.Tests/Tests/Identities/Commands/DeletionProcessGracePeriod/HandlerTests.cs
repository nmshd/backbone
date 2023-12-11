using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.DeletionProcessGracePeriod;
public class HandlerTests
{
    [Fact]
    public async Task No_Active_Deletion_Processes()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity>());

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new DeletionProcessGracePeriodCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<object>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task One_Active_First_Reminder()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeIdentity.Devices[0].Id);

        var reminderSentDate = deletionProcess.GracePeriodEndsAt!.Value
            .AddDays(-IdentityDeletionConfiguration.GracePeriodNotification1.Time);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { activeIdentity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new DeletionProcessGracePeriodCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappened();
        A.CallTo(() => mockIdentitiesRepository.Update(activeIdentity, A<CancellationToken>._))
            .MustHaveHappened();
    }

    [Fact]
    public async Task One_Active_Second_Reminder()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeIdentity.Devices[0].Id);

        var reminderSentDate = deletionProcess.GracePeriodEndsAt!.Value
            .AddDays(-IdentityDeletionConfiguration.GracePeriodNotification2.Time);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { activeIdentity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new DeletionProcessGracePeriodCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappened();
        A.CallTo(() => mockIdentitiesRepository.Update(activeIdentity, A<CancellationToken>._))
            .MustHaveHappened();

        deletionProcess.GracePeriodReminder2SentAt.Should().Be(reminderSentDate);
    }

    [Fact]
    public async Task One_Active_Third_Reminder()
    {
        // Arrange
        var beginProcessDate = DateTime.Parse("2000-01-01");
        SystemTime.Set(beginProcessDate);

        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeIdentity.Devices[0].Id);

        var reminderSentDate = deletionProcess.GracePeriodEndsAt!.Value
            .AddDays(-IdentityDeletionConfiguration.GracePeriodNotification3.Time);
        SystemTime.Set(reminderSentDate);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity> { activeIdentity });

        var handler = CreateHandler(mockIdentitiesRepository, mockPushNotificationSender);

        // Act
        await handler.Handle(new DeletionProcessGracePeriodCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.FindAllWithActiveDeletionProcess(A<CancellationToken>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => mockPushNotificationSender.SendNotification(A<IdentityAddress>._, A<DeletionProcessGracePeriodNotification>._, A<CancellationToken>._))
            .MustHaveHappened();
        A.CallTo(() => mockIdentitiesRepository.Update(activeIdentity, A<CancellationToken>._))
            .MustHaveHappened();

        deletionProcess.GracePeriodReminder3SentAt.Should().Be(reminderSentDate);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
