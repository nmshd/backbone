using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;

public class ListExternalEventsResponse(IEnumerable<ExternalEvent> items) : EnumerableResponseBase<ExternalEvent>(items);
