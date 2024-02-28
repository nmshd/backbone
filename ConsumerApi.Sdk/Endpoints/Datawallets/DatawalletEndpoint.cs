using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;

public class DatawalletEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<Datawallet>> GetDatawallet() => await _client.Get<Datawallet>("Datawallet");

    public async Task<ConsumerApiResponse<List<DatawalletModification>>> GetDatawalletModifications(int localIndex, int supportedDatawalletVersion, PaginationFilter? pagination)
        => await _client.Request<List<DatawalletModification>>(HttpMethod.Get, "Datawallet/Modifications")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("localIndex", localIndex.ToString())
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();

    public async Task<ConsumerApiResponse<PushDatawalletModificationsResponse>> PushDatawalletModifications(PushDatawalletModificationsRequest request, int supportedDatawalletVersion)
        => await _client.Request<PushDatawalletModificationsResponse>(HttpMethod.Post, "Datawallet/Modifications")
            .Authenticate()
            .WithJson(request)
            .AddExtraHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
}
