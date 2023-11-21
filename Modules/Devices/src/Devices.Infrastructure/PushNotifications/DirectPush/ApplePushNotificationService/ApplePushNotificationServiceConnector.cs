using System.Reflection;
using System.Text.Json;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApplePushNotificationServiceConnector : IPnsConnector
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;
    private readonly DirectPnsCommunicationOptions.ApnsOptions _options;

    public ApplePushNotificationServiceConnector(IHttpClientFactory httpClientFactory, IOptions<DirectPnsCommunicationOptions.ApnsOptions> options, IJwtGenerator jwtGenerator,
        ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        var sendResults = new SendResults();
        var tasks = registrations.Select(async pnsRegistration =>
        {
            ValidateRegistration(pnsRegistration);
            var handle = pnsRegistration.Handle.Value;
            var keyInformation = _options.GetKeyInformationForBundleId(pnsRegistration.AppId!);
            var jwt = _jwtGenerator.Generate(keyInformation.PrivateKey, keyInformation.KeyId, keyInformation.TeamId, pnsRegistration.AppId);

            var request = new ApnsMessageBuilder(pnsRegistration.AppId, BuildUrl(pnsRegistration.Environment, handle), jwt.Value)
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetNotificationId(notificationId)
                .Build();

            _logger.LogDebug("Sending push notification (type '{eventName}') to '{address}' with handle '{handle}'.", notificationContent.EventName, recipient, pnsRegistration.Handle);

            var response = await _httpClient.SendAsync(request);
            await MapResponse(response, pnsRegistration, sendResults);
        }).ToList();

        await Task.WhenAll(tasks);
        return sendResults;
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForBundleId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedBundleIds()));
    }

    private async Task MapResponse(HttpResponseMessage response, PnsRegistration registration, SendResults sendResults)
    {
        if (response.IsSuccessStatusCode)
            sendResults.AddSuccess(registration.DeviceId);
        else
        {
            var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            if (responseContent.reason == "Unregistered")
                sendResults.AddFailure(registration.DeviceId, ErrorReason.InvalidHandle);
            else
                sendResults.AddFailure(registration.DeviceId, ErrorReason.Unexpected, responseContent.Reason);
        }
    }

    private static string BuildUrl(PushEnvironment environment, string handle)
    {
        var baseUrl = environment switch
        {
            PushEnvironment.Development => "https://api.sandbox.push.apple.com:443/3/device",
            PushEnvironment.Production => "https://api.push.apple.com:443/3/device",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{baseUrl}/{handle}";
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }

    private static (string Title, string Body) GetNotificationText(object pushNotification)
    {
        switch (pushNotification)
        {
            case null:
                return ("", "");
            case JsonElement jsonElement:
            {
                var notification = jsonElement.Deserialize<NotificationTextAttribute>();
                return notification == null ? ("", "") : (notification.Title, notification.Body);
            }
            default:
            {
                var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
                return attribute == null ? ("", "") : (attribute.Title, attribute.Body);
            }
        }
    }
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T)type.GetCustomAttribute(typeof(T));
    }
}
