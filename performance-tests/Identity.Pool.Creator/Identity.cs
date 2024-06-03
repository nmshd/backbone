using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;
    public List<string> DeviceIds { get; }  = [];

    public Identity(UserCredentials userCredentials, string deviceId)
    {
        UserCredentials = userCredentials;
        DeviceIds.Add(deviceId);
    }

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }
}
