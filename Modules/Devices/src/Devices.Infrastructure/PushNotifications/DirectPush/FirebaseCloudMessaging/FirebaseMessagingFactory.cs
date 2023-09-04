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
        var firebaseApp = FirebaseApp.GetInstance(appId);
        if (firebaseApp == null)
        {
            var serviceAccount = _options.GetServiceAccountForAppId(appId);
            firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(serviceAccount)
            }, appId);
        }

        return FirebaseMessaging.GetMessaging(firebaseApp);
    }
}
