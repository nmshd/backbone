﻿using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;

public class DatawalletEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<Datawallet>> GetDatawallet() => await _client.Get<Datawallet>("Datawallet");

    public async Task<ConsumerApiResponse<GetDatawalletModificationsResponse>> GetDatawalletModifications(int localIndex, int supportedDatawalletVersion, PaginationFilter? pagination = null)
        => await _client.Request<GetDatawalletModificationsResponse>(HttpMethod.Get, "Datawallet/Modifications")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("localIndex", localIndex.ToString())
            .AddHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();

    public async Task<ConsumerApiResponse<PushDatawalletModificationsResponse>> PushDatawalletModifications(PushDatawalletModificationsRequest request, int supportedDatawalletVersion)
        => await _client.Request<PushDatawalletModificationsResponse>(HttpMethod.Post, "Datawallet/Modifications")
            .Authenticate()
            .WithJson(request)
            .AddHeader("x-Supported-Datawallet-Version", supportedDatawalletVersion.ToString())
            .Execute();
}
