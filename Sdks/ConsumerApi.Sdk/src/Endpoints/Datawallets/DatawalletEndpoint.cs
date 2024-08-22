using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;

public class DatawalletEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<Datawallet>> GetDatawallet()
    {
        return await _client.Get<Datawallet>("Datawallet");
    }

    public async Task<ApiResponse<CreateDatawalletResponse>> CreateDatawallet(int supportedDatawalletVersion)
    {
        return await _client.Request<CreateDatawalletResponse>(HttpMethod.Post, $"api/{API_VERSION}/Datawallet")
            .Authenticate()
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
    }

    public async Task<ApiResponse<GetDatawalletModificationsResponse>> GetDatawalletModifications(int localIndex, int supportedDatawalletVersion, PaginationFilter? pagination = null)
    {
        return await _client.Request<GetDatawalletModificationsResponse>(HttpMethod.Get, $"api/{API_VERSION}/Datawallet/Modifications")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("localIndex", localIndex.ToString())
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
    }

    public async Task<ApiResponse<PushDatawalletModificationsResponse>> PushDatawalletModifications(PushDatawalletModificationsRequest request, int supportedDatawalletVersion)
    {
        return await _client.Request<PushDatawalletModificationsResponse>(HttpMethod.Post, $"api/{API_VERSION}/Datawallet/Modifications")
            .Authenticate()
            .WithJson(request)
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
    }
}
