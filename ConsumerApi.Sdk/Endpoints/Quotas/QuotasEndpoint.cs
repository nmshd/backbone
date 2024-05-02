using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas;

public class QuotasEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<ListIndividualQuotasResponse>> ListIndividualQuotas() => await _client.Get<ListIndividualQuotasResponse>($"api/{API_VERSION}/Quotas");
}
