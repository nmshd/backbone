using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;
    private readonly List<string> _deviceIds = [];

    public Identity(UserCredentials userCredentials, string deviceId)
    {
        UserCredentials = userCredentials;
        _deviceIds.Add(deviceId);
    }

    public void AddDevice(string deviceId)
    {
        _deviceIds.Add(deviceId);
    }
}
