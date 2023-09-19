using System.Reflection;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling.Extensions;
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
        ILogger<ApplePushNotificationServiceConnector> logger, IPnsRegistrationRepository registrationRepository)
    {
        _httpClient = httpClientFactory.CreateClient();
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<SendResult>> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, object notification)
    {
        var (notificationTitle, notificationBody) = GetNotificationText(notification);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(recipient, notification);

        var tasks = registrations.Select(async pnsRegistration =>
        {
            ValidateRegistration(pnsRegistration);
            var handle = pnsRegistration.Handle.Value;
            var bundle = _options.GetBundleById(pnsRegistration.AppId!);
            var keyInformation = _options.GetKeyInformationForBundleId(pnsRegistration.AppId!);
            var jwt = _jwtGenerator.Generate(keyInformation.PrivateKey, keyInformation.KeyId, keyInformation.TeamId, pnsRegistration.AppId);

            var request = new ApnsMessageBuilder(pnsRegistration.AppId, $"{bundle.Server}{handle}", jwt.Value)
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetNotificationId(notificationId)
                .Build();

            _logger.LogDebug("Sending push notification (type '{eventName}') to '{address}' with handle '{handle}'.", notificationContent.EventName, recipient, pnsRegistration.Handle);

            var response = await _httpClient.SendAsync(request);
            return await MapResponse(response, pnsRegistration);
        }).ToList();

        return (await Task.WhenAll(tasks)).ToList();
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForBundleId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedBundleIds()));
    }

    private async Task<SendResult> MapResponse(HttpResponseMessage response, PnsRegistration registration)
    {
        if (response.IsSuccessStatusCode) return SendResult.Success();

        var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        string reason = responseContent.reason;
        if (!reason.IsNullOrEmpty())
        {
            if (reason == "Unregistered")
            {
                return SendResult.Failure(registration.DeviceId, SendResult.FailureReason.InvalidHandle);
            }
        }

        return SendResult.Failure(registration.DeviceId, SendResult.FailureReason.Unexpected, responseContent.Reason);
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
