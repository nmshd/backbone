using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Microsoft.IdentityModel.Tokens;

namespace Backbone.Modules.Devices.Application.Devices.DTOs;

public class PublicKey
{
    public PublicKey(SignatureAlgorithm algorithm, byte[] key)
    {
        Algorithm = algorithm;
        Key = key;
    }

    public SignatureAlgorithm Algorithm { get; set; }
    public byte[] Key { get; set; }

    public byte[] ToBytes()
    {
        var obj = new
        {
            pub = Key,
            alg = Algorithm
        };
        var publicKeyJsonString = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(publicKeyJsonString);

        return bytes;
    }

    public static PublicKey FromBytes(byte[] bytes)
    {
        var publicKeyJsonString = Encoding.UTF8.GetString(bytes);
        var publicKeyObject =
            JsonSerializer.Deserialize<dynamic>(
                publicKeyJsonString,
                new JsonSerializerOptions { Converters = { new DynamicJsonConverter() } }) ??
            throw new Exception("Could not deserialize public key.");
        var key = Base64UrlEncoder.DecodeBytes((string)publicKeyObject.pub);
        var algorithm = (SignatureAlgorithm)publicKeyObject.alg;

        return new PublicKey(algorithm, key);
    }

    public class PublicKeyDTOJsonConverter : JsonConverter<PublicKey>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PublicKey);
        }

        public override PublicKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rawBytes = reader.GetBytesFromBase64();

            return FromBytes(rawBytes);
        }

        public override void Write(Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
        {
            writer.WriteBase64StringValue(value.ToBytes());
        }
    }
}
