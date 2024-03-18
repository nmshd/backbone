using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types.Responses;

public class ListTiersResponse(IEnumerable<TierOverview> items) : EnumerableResponseBase<TierOverview>(items);
