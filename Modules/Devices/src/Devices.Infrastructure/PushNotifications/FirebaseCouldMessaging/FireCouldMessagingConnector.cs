using System.Net.Http.Headers;
using System.Text;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCouldMessaging;
public class FireCouldMessagingConnector : IPnsConnector
{
    private readonly string _apiKey;
    private readonly HttpClient _client;

    public FireCouldMessagingConnector(IOptions<FireCouldMessagingConnectorContextOptions> options)
    {
        _apiKey = options.Value.APIKey;
        _client = new HttpClient();
    }

    public async Task Send(IEnumerable<PnsRegistration> registrations, object notification)
    {
        var values = new FCMMessage
        {
            Data =
            {
                AndroidChannelId = "Enmeshed",
                ContentAvailable = "1",
                Content = new()
                {
                    { "accRef", "id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"},
                    { "eventName", "dynamic"}
                }
            },
            Notification = notification,
            Recipients = registrations.Select(r=>r.Handle.Value)
         };

        var httpContent = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_apiKey);

        await _client.PostAsync("https://fcm.googleapis.com/fcm/send", httpContent);
    }
}

public sealed class FCMMessage
{
    [JsonProperty("data")]
    public FCMData Data { get; set; }

    [JsonProperty("notification")]
    public object Notification { get; set; }

    [JsonProperty("registration_ids")]
    public IEnumerable<string> Recipients { get; set; }
}

public sealed class FCMData
{
    [JsonProperty("android_channel_id")]
    public string AndroidChannelId;

    [JsonProperty("content_available")]
    public string ContentAvailable;

    [JsonProperty("content")]
    public Dictionary<string, dynamic> Content;
}

public sealed class FireCouldMessagingConnectorContextOptions
{
    public string APIKey { get; set; }
}
