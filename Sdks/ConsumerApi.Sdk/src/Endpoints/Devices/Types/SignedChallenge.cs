using System.Text;
using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.Crypto;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;

public class SignedChallenge
{
    public SignedChallenge(string challenge, ConvertibleString signature)
    {
        Challenge = challenge;
        var sig = new CryptoSignatureSignedChallenge { alg = CryptoHashAlgorithm.SHA512, sig = signature.BytesRepresentation };
        var options = new JsonSerializerOptions { IncludeFields = true };
        var json = JsonSerializer.Serialize(sig, options);
        Signature = ConvertibleString.FromByteArray(Encoding.UTF8.GetBytes(json)).Base64Representation;
    }

    public string Challenge { get; internal set; }
    public string Signature { get; internal set; }
}
