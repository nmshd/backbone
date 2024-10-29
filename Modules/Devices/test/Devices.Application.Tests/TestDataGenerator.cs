using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Application.Tests;

public static class TestDataGenerator
{
    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }

    public static Identity CreateIdentity()
    {
        return new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    public static Identity CreateIdentityWithTier(TierId tierId)
    {
        return new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            tierId,
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    public static Identity CreateIdentityWithOneDevice()
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));

        return identity;
    }

    public static IdentityDeletionProcess CreateCancelledDeletionProcessFor(Identity identity)
    {
        var deletionProcess = identity.StartDeletionProcessAsSupport();
        identity.ApproveDeletionProcess(deletionProcess.Id, identity.Devices.First().Id);
        identity.CancelDeletionProcessAsSupport(deletionProcess.Id);

        return deletionProcess;
    }

    public static IdentityDeletionProcess CreateApprovedDeletionProcessFor(Identity identity, DeviceId deviceId)
    {
        var deletionProcess = identity.StartDeletionProcessAsOwner(deviceId);

        return deletionProcess;
    }

    public static IdentityDeletionProcess CreateRejectedDeletionProcessFor(Identity identity, DeviceId deviceId)
    {
        var deletionProcess = identity.StartDeletionProcessAsSupport();
        identity.RejectDeletionProcess(deletionProcess.Id, deviceId);

        return deletionProcess;
    }

    public static IdentityDeletionProcess CreateDeletingDeletionProcessFor(Identity identity, DeviceId deviceId)
    {
        var deletionProcess = identity.StartDeletionProcessAsOwner(deviceId);

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays));
        identity.DeletionStarted();
        SystemTime.UndoSet();

        return deletionProcess;
    }

    public static Identity CreateIdentityWithApprovedDeletionProcess(DateTime? approvalDate = null)
    {
        approvalDate ??= SystemTime.UtcNow;

        var identity = CreateIdentityWithOneDevice();

        SystemTime.Set(approvalDate.Value);
        identity.StartDeletionProcessAsOwner(identity.Devices[0].Id);
        SystemTime.UndoSet();

        return identity;
    }

    public static Identity CreateIdentityWithDeletionProcessWaitingForApproval(DateTime deletionProcessStartedAt)
    {
        var identity = CreateIdentityWithOneDevice();

        SystemTime.Set(deletionProcessStartedAt);
        identity.StartDeletionProcessAsSupport();
        SystemTime.UndoSet();

        return identity;
    }
}
