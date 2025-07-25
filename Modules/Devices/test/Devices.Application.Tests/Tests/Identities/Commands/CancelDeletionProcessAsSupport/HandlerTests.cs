using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsSupport;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(utcNow);
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => fakeIdentitiesRepository
                .Get(identity.Address, A<CancellationToken>._, true))
            .Returns(identity);

        var handler = CreateHandler(fakeIdentitiesRepository, mockPushNotificationSender);

        // Acting
        var result = await handler.Handle(new CancelDeletionAsSupportCommand { Address = identity.Address, DeletionProcessId = deletionProcess.Id }, CancellationToken.None);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Active);

        result.Id.ShouldBe(deletionProcess.Id);
        result.Status.ShouldBe(DeletionProcessStatus.Cancelled);
        result.CancelledAt.ShouldBe(utcNow);

        A.CallTo(() => mockPushNotificationSender.SendNotification(
            A<DeletionProcessCancelledBySupportPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(identity.Address)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Get(identity.Address, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);
        var deletionProcessId = IdentityDeletionProcessId.Generate();
        var handler = CreateHandler(identitiesRepository);

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionAsSupportCommand { Address = identity.Address, DeletionProcessId = deletionProcessId }, CancellationToken.None);

        // Assert
        var exception = await acting.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldContain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IPushNotificationSender? pushNotificationSender = null)
    {
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();

        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
