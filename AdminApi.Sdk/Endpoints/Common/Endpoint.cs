namespace Backbone.AdminApi.Sdk.Endpoints.Common;

public abstract class Endpoint(EndpointClient client)
{
    protected EndpointClient _client = client;
}
