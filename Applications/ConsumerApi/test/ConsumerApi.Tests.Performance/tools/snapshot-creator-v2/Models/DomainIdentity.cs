using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record DomainIdentity(
    UserCredentials UserCredentials,
    string IdentityAddress, // the real identity address returned by sdk 
    string DeviceId,
    IdentityPoolConfiguration IdentityPoolConfiguration,
    int IdentityConfigurationAddress, // the address from pool-config json 
    int NumberOfDevices)
{
    public List<string> DeviceIds = [];

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }
}
