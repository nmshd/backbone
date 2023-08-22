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
            Credential = _options.KeysByApplicationId.GetValueOrDefault(appId)?.ServiceAccountJson is null
                ? GoogleCredential.GetApplicationDefault()
                : GoogleCredential.FromJson(_options.KeysByApplicationId[appId].ServiceAccountJson)
        });

        return FirebaseMessaging.GetMessaging(firebaseApp);
    }
}
