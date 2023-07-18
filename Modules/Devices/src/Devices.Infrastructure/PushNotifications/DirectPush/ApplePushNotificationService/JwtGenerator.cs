using System.Security.Cryptography;
using Enmeshed.Tooling;
using JWT.Algorithms;
using JWT.Builder;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class JwtGenerator : IJwtGenerator
{
    public Jwt Generate(string privateKey, string keyId, string teamId)
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

public class Jwt
{
    public readonly string Value;
    private readonly DateTime _createdAt;

    public Jwt(string value)
    {
        Value = value;
        _createdAt = SystemTime.UtcNow;
    }

    public bool IsExpired()
    {
        return _createdAt.AddMinutes(50) < SystemTime.UtcNow;
    }
}