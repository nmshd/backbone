using Backbone.BuildingBlocks.Domain.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
    private readonly NotificationTextService _notificationTextService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;
    private readonly DirectPnsCommunicationOptions.ApnsOptions _options;

    public ApplePushNotificationServiceConnector(IHttpClientFactory httpClientFactory, IOptions<DirectPnsCommunicationOptions.ApnsOptions> options, IJwtGenerator jwtGenerator,
        NotificationTextService notificationTextService,
        ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _jwtGenerator = jwtGenerator;
        _notificationTextService = notificationTextService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, IPushNotification notification)
    {
        ValidateRegistrations(registrations);

        var sendResults = new SendResults();

        var tasks = registrations.Select(r => SendNotification(r, notification, sendResults));

        await Task.WhenAll(tasks);
        return sendResults;
    }

    private void ValidateRegistrations(IEnumerable<PnsRegistration> registrations)
    {
        foreach (var registration in registrations)
        {
            ValidateRegistration(registration);
        }
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_options.HasConfigForBundleId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_options.GetSupportedBundleIds()));
    }

    private async Task SendNotification(PnsRegistration registration, IPushNotification notification, SendResults sendResults)
    {
        var (notificationTitle, notificationBody) = await _notificationTextService.GetNotificationTextForDeviceId(notification.GetType(), registration.DeviceId);
        var notificationId = GetNotificationId(notification);
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);

        var keyInformation = _options.GetKeyInformationForBundleId(registration.AppId);
        var jwt = _jwtGenerator.Generate(keyInformation.PrivateKey, keyInformation.KeyId, keyInformation.TeamId, registration.AppId);

        var request = new ApnsMessageBuilder(registration.AppId, BuildUrl(registration.Environment, registration.Handle.Value), jwt.Value)
                .AddContent(notificationContent)
                .SetNotificationText(notificationTitle, notificationBody)
                .SetNotificationId(notificationId)
                .Build();

        _logger.LogDebug("Sending push notification (type '{eventName}') to '{address}' with handle '{handle}'.", notificationContent.EventName, registration.IdentityAddress, registration.Handle);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            sendResults.AddSuccess(registration.DeviceId);
        else
        {
            var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync())!;
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
}
