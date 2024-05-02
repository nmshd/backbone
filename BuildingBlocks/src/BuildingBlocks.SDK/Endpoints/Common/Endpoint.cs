namespace Backbone.BuildingBlocks.SDK.Endpoints.Common;

public abstract class Endpoint(EndpointClient client)
{
    protected const string API_VERSION = "v1";

    protected readonly EndpointClient _client = client;
}
