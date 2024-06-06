using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;
    public List<string> DeviceIds { get; }  = [];
    public string Address { private set; get; }

    public bool HasBeenUsedAsMessageRecipient { get; private set; }

    public Identity(UserCredentials userCredentials, string address, string deviceId)
    {
        Address = address;
        UserCredentials = userCredentials;
        DeviceIds.Add(deviceId);
        HasBeenUsedAsMessageRecipient = false;
    }

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public void UseAsMessageRecipient()
    {
        HasBeenUsedAsMessageRecipient = true;
    }
}
