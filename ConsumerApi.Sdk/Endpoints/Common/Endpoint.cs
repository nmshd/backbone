namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

public abstract class Endpoint
{
    protected readonly EndpointClient _client;

    protected Endpoint(EndpointClient client)
    {
        _client = client;
    }
}
