using System.Net;
using System.Net.Http.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public interface ISseServerClient
{
    Task SendEvent(string recipient, string eventName);
}

public class SseServerClient : ISseServerClient
{
    private readonly HttpClient _client;

    public SseServerClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient(nameof(SseServerClient));
    }

    public async Task SendEvent(string recipient, string eventName)
    {
        var request = new SseMessageBuilder(recipient, eventName).Build();
        var response = await _client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorPayload = await response.Content.ReadFromJsonAsync<ErrorPayload>();

            if (errorPayload is { Code: "error.platform.sseClientNotRegistered" })
                throw new SseClientNotRegisteredException();

            throw new Exception("An unexpected error occurred while sending the event.");
        }
    }

    private class ErrorPayload
    {
        public string? Code { get; set; }
    }
}

public class SseClientNotRegisteredException : Exception;
