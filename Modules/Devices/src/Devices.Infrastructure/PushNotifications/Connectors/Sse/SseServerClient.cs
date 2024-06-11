using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseServerClient
{
    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new()
    {
        Converters = { new DateTimeConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _client;

    public SseServerClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient(nameof(SseServerClient));
    }

    public async Task SendEvent(string recipient, string eventName)
    {
        var payload = JsonContent.Create(new EventPayload(eventName), options: JSON_SERIALIZER_OPTIONS);

        try
        {
            var response = await _client.PostAsync($"/{recipient}/events", payload);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorPayload = await response.Content.ReadFromJsonAsync<ErrorPayload>();

                if (errorPayload is { Code: "error.platform.sseClientNotRegistered" })
                    throw new SseClientNotRegisteredException();

                throw new Exception("An unexpected error occurred while sending the event.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while sending the event.", ex);
        }
    }

    private class EventPayload
    {
        public EventPayload(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; set; }
    }

    private class ErrorPayload
    {
        public string? Code { get; set; }
    }
}

public class SseClientNotRegisteredException : Exception;
