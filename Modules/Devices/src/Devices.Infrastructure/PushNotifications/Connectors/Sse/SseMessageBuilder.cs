using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseMessageBuilder
{
    private readonly string _recipient;
    private readonly string _eventName;

    public SseMessageBuilder(string recipient, string eventName)
    {
        _recipient = recipient;
        _eventName = eventName;
    }

    public HttpRequestMessage Build()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_recipient}/events");
        request.Content = JsonContent.Create(new EventPayload(_eventName));
        return request;
    }

    private class EventPayload
    {
        public EventPayload(string eventName)
        {
            EventName = eventName;
        }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; }
    }
}
