using Backbone.AdminApi.Sdk.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Clients.Types.Responses;

public class ListClientsResponse(IEnumerable<ClientOverwiew> items) : EnumerableResponseBase<ClientOverwiew>(items);
