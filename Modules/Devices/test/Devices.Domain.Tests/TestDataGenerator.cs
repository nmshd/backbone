using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests;

public static class TestDataGenerator
{
    public static Identity CreateIdentity(TierId? tierId = null)
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            tierId ?? TierId.Generate(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);

        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        identity.Devices.Add(device);

        identity.ClearDomainEvents();

        return identity;
    }

    public static Identity CreateIdentityWithoutDevice(TierId? tierId = null)
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            tierId ?? TierId.Generate(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);

        identity.ClearDomainEvents();

        return identity;
    }

    public static Identity CreateIdentityWithActiveDeletionProcess()
    {
        var identity = CreateIdentity();
        identity.StartDeletionProcess(identity.Devices.First().Id);
        identity.ClearDomainEvents();

        foreach (var deletionProcess in identity.DeletionProcesses)
        {
            deletionProcess.ClearDomainEvents();
        }

        return identity;
    }
}
