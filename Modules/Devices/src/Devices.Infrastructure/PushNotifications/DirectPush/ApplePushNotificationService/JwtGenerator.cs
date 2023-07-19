using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class JwtGenerator : IJwtGenerator
{
    private const string JWT_LOCK = "JWT_LOCK";
    private Jwt _jwt;

    public Jwt Generate(string privateKey, string keyId, string teamId)
    {
        if (_jwt == null || _jwt.IsExpired())
        {
            // we cannot lock _jwt because it is possibly null here, and you cannot lock on null
            // CAUTION: don't remove this, since it would cause the JWT to be generated multiple times, and we are only allowed to generate one JWT per 20 minutes (see https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/establishing_a_token-based_connection_to_apns#2943374)
            lock (JWT_LOCK)
            {
                if (_jwt == null || _jwt.IsExpired())
                    _jwt = CreateNew(privateKey, keyId, teamId);
            }
        }

        return _jwt;
    }

    protected virtual Jwt CreateNew(string privateKey, string keyId, string teamId)
    {
        var keyParams = (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
        var q = keyParams.Parameters.G.Multiply(keyParams.D).Normalize();
        var ecdsa = ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.CreateFromValue(keyParams.PublicKeyParamSet.Id),
            D = keyParams.D.ToByteArrayUnsigned(),
            Q =
            {
                X = q.XCoord.GetEncoded(),
                Y = q.YCoord.GetEncoded()
            }
        });
        var token = JwtBuilder.Create()
            .WithAlgorithm(new ES256Algorithm(ecdsa, ecdsa))
            .AddHeader("kid", keyId)
            .AddClaim("iss", teamId)
            .AddClaim("iat", ToEpoch(DateTime.UtcNow))
            .Encode();

        return new Jwt(token);
    }

    private static int ToEpoch(DateTime time)
    {
        var span = time - new DateTime(1970, 1, 1);
        return Convert.ToInt32(span.TotalSeconds);
    }
}
