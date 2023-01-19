using Devices.Application.Infrastructure.PushNotifications;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Devices.Infrastructure.PushNotifications;

public static class IServiceCollectionExtensions
{
    public static void AddPushNotifications(this IServiceCollection services, Action<PushNotificationsOptions> setupOptions)
    {
        var options = new PushNotificationsOptions();
        setupOptions?.Invoke(options);

        services.AddTransient<IPushService>(sp =>
        {
            var client = NotificationHubClient.CreateClientFromConnectionString(options.ConnectionString, options.HubName);
            var logger = sp.GetService<ILogger<AzureNotificationHubPushService>>();

            return new AzureNotificationHubPushService(client, logger);
        });
    }

    public class PushNotificationsOptions
    {
        public string ConnectionString { get; set; }
        public string HubName { get; set; }
    }
}

public static class NotificationHubClientExtensions
{
    public static async Task<List<string>> GetAllInstallations(this NotificationHubClient client)
    {
        var allRegistrations = await client.GetAllRegistrationsAsync(0);
        var continuationToken = allRegistrations.ContinuationToken;
        var registrationDescriptionsList = new List<RegistrationDescription>(allRegistrations);
        while (!string.IsNullOrWhiteSpace(continuationToken))
        {
            var otherRegistrations = await client.GetAllRegistrationsAsync(continuationToken, 0);
            registrationDescriptionsList.AddRange(otherRegistrations);
            continuationToken = otherRegistrations.ContinuationToken;
        }

        var deviceInstallationList = new List<string>();

        foreach (var registration in registrationDescriptionsList)
        {
            var installationId = "";

            foreach (var tag in registration.Tags)
            {
                if (tag.Contains("InstallationId:"))
                    installationId = new Guid(tag[(tag.IndexOf(":", StringComparison.InvariantCulture) + 1)..]).ToString();
            }

            deviceInstallationList.Add(installationId);
        }

        return deviceInstallationList;
    }

    public static async Task DeleteAllInstallations(this NotificationHubClient client)
    {
        foreach (var installation in await client.GetAllInstallations())
        {
            await client.DeleteInstallationAsync(installation);
        }
    }
}
