using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types.Responses;

public class ListIndividualQuotasResponse(IEnumerable<QuotaGroup> items) : EnumerableResponseBase<QuotaGroup>(items);
