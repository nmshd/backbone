using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record DomainIdentity(
    UserCredentials DeviceDataUserCredentials,
    string DeviceDataDeviceId,
    string DeviceId,
    IdentityPoolConfiguration IdentityPoolConfiguration,
    int IdentityConfigurationAddress)
{
    public List<string> DeviceIds = [];

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }
}
