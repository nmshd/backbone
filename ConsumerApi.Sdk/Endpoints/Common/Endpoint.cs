namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

public abstract class Endpoint(EndpointClient client)
{
    protected readonly EndpointClient _client = client;
}
