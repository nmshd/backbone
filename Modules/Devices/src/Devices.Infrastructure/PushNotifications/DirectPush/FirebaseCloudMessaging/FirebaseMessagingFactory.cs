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
        var firebaseApp = FirebaseApp.GetInstance(appId) ?? RegisterNewInstance(appId);

        return FirebaseMessaging.GetMessaging(firebaseApp);
    }

    private FirebaseApp RegisterNewInstance(string appId)
    {
        var serviceAccount = _options.GetServiceAccountForAppId(appId);
        var credential = GoogleCredential.FromJson(serviceAccount);
        var firebaseApp = FirebaseApp.Create(new AppOptions { Credential = credential }, appId);

        return firebaseApp;
    }
}
