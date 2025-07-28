using System.Text.Json;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;

public class ApplePushNotificationServiceConnector : IPnsConnector
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;
    private readonly ApnsConfiguration _configuration;

    public ApplePushNotificationServiceConnector(IHttpClientFactory httpClientFactory, IOptions<ApnsConfiguration> options, IJwtGenerator jwtGenerator,
        ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _configuration = options.Value;
    }

    public async Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText)
    {
        var notificationContent = new NotificationContent(registration.IdentityAddress, registration.DevicePushIdentifier, notification);
        var notificationId = GetNotificationId(notification);

        return await Send(registration, notificationText, notificationContent, notificationId);
    }

    private static string? GetNotificationId(object? pushNotification)
    {
        var attribute = pushNotification?.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value;
    }

    public async Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, string notificationId)
    {
        return await Send(registration, notificationText, null, notificationId);
    }

    private async Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, NotificationContent? notificationContent, string? notificationId)
    {
        ValidateRegistration(registration);

        var jwt = GetJwt(registration);

        var messageBuilder = new ApnsMessageBuilder(registration.AppId, registration.Environment, (ApnsHandle)registration.Handle, jwt.Value)
            .SetNotificationText(notificationText.Title, notificationText.Body)
            .SetNotificationId(notificationId)
            .AddContent(notificationContent);

        var message = messageBuilder.Build();

        _logger.Sending();

        return await Send(registration, message);
    }

    private Jwt GetJwt(PnsRegistration registration)
    {
        var keyInformation = _configuration.GetKeyInformationForBundleId(registration.AppId);
        var jwt = _jwtGenerator.Generate(keyInformation.PrivateKey, keyInformation.KeyId, keyInformation.TeamId, registration.AppId);
        return jwt;
    }

    private async Task<SendResult> Send(PnsRegistration registration, HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return SendResult.Success(registration.DeviceId);

        var responseContent = await JsonSerializer.DeserializeAsync<ApiError>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions.Web);

        if (responseContent?.Reason == "Unregistered")
            return SendResult.Failure(registration.DeviceId, ErrorReason.InvalidHandle);

        return SendResult.Failure(registration.DeviceId, ErrorReason.Unexpected, responseContent?.Reason);
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        if (!_configuration.HasConfigForBundleId(registration.AppId))
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(_configuration.GetSupportedBundleIds()));
    }
}

file record ApiError
{
    public string? Reason { get; set; }
}

internal static partial class ApplePushNotificationServiceConnectorLogs
{
    [LoggerMessage(
        EventId = 770700,
        EventName = "ApplePushNotificationServiceConnector.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification').")]
    public static partial void Sending(this ILogger logger);
}
