using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas;

public class QuotasEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<ListIndividualQuotasResponse>> ListIndividualQuotas() => await _client.Get<ListIndividualQuotasResponse>("Quotas");
}
