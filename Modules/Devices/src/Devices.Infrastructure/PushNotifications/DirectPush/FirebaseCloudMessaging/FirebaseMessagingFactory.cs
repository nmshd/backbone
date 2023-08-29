using Backbone.Modules.Devices.Application.PushNotifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;

public class FirebaseMessagingFactory
{
    private readonly DirectPnsCommunicationOptions.FcmOptions _options;

    public FirebaseMessagingFactory(IOptions<DirectPnsCommunicationOptions.FcmOptions> options)
    {
        _options = options.Value;
    }

    public FirebaseMessaging CreateForAppId(string appId)
    {
        var firebaseApp = FirebaseApp.GetInstance(appId) ?? FirebaseApp.Create(new AppOptions
        {
            Credential = _options.ServiceAccounts.GetValueOrDefault(_options.Apps.GetValueOrDefault(appId)?.ServiceAccountName).ServiceAccountJson is null
                ? GoogleCredential.GetApplicationDefault()
                : GoogleCredential.FromJson(_options.ServiceAccounts[_options.Apps[appId].ServiceAccountName].ServiceAccountJson)
        });

        return FirebaseMessaging.GetMessaging(firebaseApp);
    }
}
