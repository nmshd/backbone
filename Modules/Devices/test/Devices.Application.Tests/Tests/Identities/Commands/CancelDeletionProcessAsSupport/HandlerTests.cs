using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
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
        var result = await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address, deletionProcess.Id), CancellationToken.None);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        result.Id.Should().Be(deletionProcess.Id);
        result.Status.Should().Be(DeletionProcessStatus.Cancelled);
        result.CancelledAt.Should().Be(utcNow);

        A.CallTo(() => mockPushNotificationSender.SendNotification(
            A<DeletionProcessCancelledBySupportPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(identity.Address)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcessId = IdentityDeletionProcessId.Generate();
        var handler = CreateHandler();

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address, deletionProcessId), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, CancelDeletionAsSupportResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IPushNotificationSender? pushNotificationSender = null)
    {
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();

        return new Handler(identitiesRepository, pushNotificationSender);
    }
}
