using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;

public class DatawalletEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<Datawallet>> GetDatawallet() => await _client.Get<Datawallet>("Datawallet");

    public async Task<ApiResponse<GetDatawalletModificationsResponse>> GetDatawalletModifications(int localIndex, int supportedDatawalletVersion, PaginationFilter? pagination = null)
        => await _client.Request<GetDatawalletModificationsResponse>(HttpMethod.Get, "Datawallet/Modifications")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("localIndex", localIndex.ToString())
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();

    public async Task<ApiResponse<PushDatawalletModificationsResponse>> PushDatawalletModifications(PushDatawalletModificationsRequest request, int supportedDatawalletVersion)
        => await _client.Request<PushDatawalletModificationsResponse>(HttpMethod.Post, "Datawallet/Modifications")
            .Authenticate()
            .WithJson(request)
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
}
