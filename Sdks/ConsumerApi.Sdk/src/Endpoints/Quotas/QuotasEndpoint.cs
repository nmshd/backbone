﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas;

public class QuotasEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListIndividualQuotasResponse>> ListIndividualQuotas()
    {
        return await _client.Get<ListIndividualQuotasResponse>($"api/{API_VERSION}/Quotas");
    }
}
