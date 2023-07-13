using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class Jwt
{
    public readonly string Value;
    private readonly DateTime _expirationDate;
    private const string ENCODING_ALGORITHM = "ES256";
    private const int TOKEN_EXPIRES_IN = 50;

    public Jwt(string value, DateTime expirationDate)
    {
        Value = value;
        _expirationDate = expirationDate;
    }

    private static ECDsa GetEllipticCurveAlgorithm(string privateKey)
    {
        var keyParams = (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
        var q = keyParams.Parameters.G.Multiply(keyParams.D).Normalize();

        return ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.CreateFromValue(keyParams.PublicKeyParamSet.Id),
            D = keyParams.D.ToByteArrayUnsigned(),
            Q =
            {
                X = q.XCoord.GetEncoded(),
                Y = q.YCoord.GetEncoded()
            }
        });
    }

    public static Jwt Create(string privateKey, string keyId, string teamId)
    {
        var header = JsonConvert.SerializeObject(new { alg = ENCODING_ALGORITHM, kid = keyId });
        var payload = JsonConvert.SerializeObject(new { iss = teamId, iat = ToEpoch(DateTime.UtcNow) });
        var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
        var unsignedJwtBytes = Encoding.UTF8.GetBytes(unsignedJwtData);

        using var ecDsa = GetEllipticCurveAlgorithm(privateKey);
        var signature = ecDsa.SignData(unsignedJwtBytes, 0, unsignedJwtBytes.Length, HashAlgorithmName.SHA256);
        return new Jwt($"{unsignedJwtData}.{Convert.ToBase64String(signature)}", DateTime.UtcNow.AddMinutes(-TOKEN_EXPIRES_IN));
    }

    private static int ToEpoch(DateTime time)
    {
        var span = time - new DateTime(1970, 1, 1);
        return Convert.ToInt32(span.TotalSeconds);
    }
}