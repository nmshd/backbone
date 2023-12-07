using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
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

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
