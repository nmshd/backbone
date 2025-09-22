using Backbone.BuildingBlocks.SDK.Endpoints.Common;

namespace Backbone.ConsumerApi.Sdk.Endpoints;

public abstract class ConsumerApiEndpoint : Endpoint
{
    protected const string API_VERSION = "v2";

    protected ConsumerApiEndpoint(EndpointClient client) : base(client)
    {
    }
}
