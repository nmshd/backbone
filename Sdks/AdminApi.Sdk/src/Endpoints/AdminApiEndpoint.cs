using Backbone.BuildingBlocks.SDK.Endpoints.Common;

namespace Backbone.AdminApi.Sdk.Endpoints;

public abstract class AdminApiEndpoint : Endpoint
{
    protected const string API_VERSION = "v1";

    protected AdminApiEndpoint(EndpointClient client) : base(client)
    {
    }
}
