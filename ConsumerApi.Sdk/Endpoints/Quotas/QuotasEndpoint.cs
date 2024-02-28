using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas;

public class QuotasEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<List<QuotaGroup>>> ListIndividualQuotas() => await _client.Get<List<QuotaGroup>>("Quotas");
}
