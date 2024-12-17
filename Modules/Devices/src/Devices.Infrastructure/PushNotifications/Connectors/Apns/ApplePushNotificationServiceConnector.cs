using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;

public class ApplePushNotificationServiceConnector : IPnsConnector
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;
    private readonly ApnsOptions _options;

    public ApplePushNotificationServiceConnector(IHttpClientFactory httpClientFactory, IOptions<ApnsOptions> options, IJwtGenerator jwtGenerator, ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText)
    {
        ValidateRegistration(registration);

        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        var keyInformation = _options.GetKeyInformationForBundleId(registration.AppId);
        var jwt = _jwtGenerator.Generate(keyInformation.PrivateKey, keyInformation.KeyId, keyInformation.TeamId, registration.AppId);

        var request = new ApnsMessageBuilder(registration.AppId, BuildUrl(registration.Environment, registration.Handle.Value), jwt.Value)
            .AddContent(notificationContent)
            .SetNotificationText(notificationText.Title, notificationText.Body)
            .SetNotificationId(notificationId)
            .Build();

        _logger.Sending(notificationContent.EventName);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return SendResult.Success(registration.DeviceId);

        var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync())!;

        if (responseContent.reason == "Unregistered")
            return SendResult.Failure(registration.DeviceId, ErrorReason.InvalidHandle);

        return SendResult.Failure(registration.DeviceId, ErrorReason.Unexpected, responseContent.Reason);
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForBundleId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedBundleIds()));
    }

    private static string BuildUrl(PushEnvironment environment, string handle)
    {
        var baseUrl = environment switch
        {
            PushEnvironment.Development => "https://api.sandbox.push.apple.com:443/3/device",
            PushEnvironment.Production => "https://api.push.apple.com:443/3/device",
            _ => throw new ArgumentOutOfRangeException(nameof(environment))
        };

        return $"{baseUrl}/{handle}";
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }
}

internal static partial class ApplePushNotificationServiceConnectorLogs
{
    [LoggerMessage(
        EventId = 770700,
        EventName = "ApplePushNotificationServiceConnector.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification (type '{eventName}').")]
    public static partial void Sending(this ILogger logger, string eventName);
}
