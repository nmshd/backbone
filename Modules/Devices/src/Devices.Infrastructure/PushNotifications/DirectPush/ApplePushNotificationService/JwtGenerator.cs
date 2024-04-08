using System.Security.Cryptography;
using Backbone.Tooling;
using JWT.Algorithms;
using JWT.Builder;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class JwtGenerator : IJwtGenerator
{
    private readonly ApnsJwtCache _jwtCache;

    public JwtGenerator(ApnsJwtCache jwtCache)
    {
        _jwtCache = jwtCache;
    }

    public Jwt Generate(string privateKey, string keyId, string teamId, string bundleId)
    {
        lock (_jwtCache)
        {
            if (!_jwtCache.HasValueFor(bundleId))
                _jwtCache.UpdateValueFor(bundleId, CreateNew(privateKey, keyId, teamId));

            return _jwtCache.GetValueFor(bundleId);
        }
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
            .AddClaim("iat", ToEpoch(SystemTime.UtcNow))
            .Encode();

        return new Jwt(token);
    }

    private static int ToEpoch(DateTime time)
    {
        var span = time - new DateTime(1970, 1, 1);
        return Convert.ToInt32(span.TotalSeconds);
    }
}
