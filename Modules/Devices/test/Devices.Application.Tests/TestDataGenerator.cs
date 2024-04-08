using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

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
            1);
    }

    public static Identity CreateIdentityWithOneDevice()
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1);
        identity.Devices.Add(new Device(identity));

        return identity;
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
