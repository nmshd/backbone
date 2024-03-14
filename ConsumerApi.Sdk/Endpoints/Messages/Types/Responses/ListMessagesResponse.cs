using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;

public class ListMessagesResponse(IEnumerable<Message> items) : EnumerableResponseBase<Message>(items);
