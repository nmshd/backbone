using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

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
            1);

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
            1);

        identity.ClearDomainEvents();

        return identity;
    }

    public static string GenerateString(int resultLength, char[]? chars = null)
    {
        chars ??= ['A', 'B', 'C'];

        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = CreateIdentity();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);
        identity.ClearDomainEvents();

        foreach (var deletionProcess in identity.DeletionProcesses)
        {
            deletionProcess.ClearDomainEvents();
        }

        return identity;
    }

    public static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        identity.StartDeletionProcessAsSupport();

        foreach (var deletionProcess in identity.DeletionProcesses)
        {
            deletionProcess.ClearDomainEvents();
        }

        return identity;
    }
}
