using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseServerClient
{
    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new()
    {
        Converters = { new DateTimeConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _client;

    public SseServerClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient(nameof(SseServerClient));
    }

    public async Task SendEvent(string recipient, NotificationContent notificationContent)
    {
        var payload = JsonContent.Create(new EventPayload { Message = JsonSerializer.Serialize(new { notificationContent.EventName }, JSON_SERIALIZER_OPTIONS) });
        var response = await _client.PostAsync($"/{recipient}/events", payload);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorPayload = await response.Content.ReadFromJsonAsync<ErrorPayload>();

            if (errorPayload is { Code: "error.platform.sseClientNotRegistered" })
                throw new SseClientNotRegisteredException();

            throw new Exception("An unexpected error occurred while sending the event.");
        }

        // Possible errors:
        // - The server is not reachable
        // - The sse connection is closed (need to return a corresponding error from sse server)
    }

    private class EventPayload
    {
        public required string Message { get; set; }
    }

    private class ErrorPayload
    {
        public string Code { get; }
    }
}

public class SseClientNotRegisteredException : Exception;
