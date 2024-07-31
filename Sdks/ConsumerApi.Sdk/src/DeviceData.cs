using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.ConsumerApi.Sdk;

public record DeviceData
{
    public required string DeviceId;
    public required UserCredentials UserCredentials;
}
