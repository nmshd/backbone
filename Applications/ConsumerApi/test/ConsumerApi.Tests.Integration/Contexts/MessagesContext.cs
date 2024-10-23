using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class MessagesContext
{
    public readonly Dictionary<string, Message> Messages = new();
}
