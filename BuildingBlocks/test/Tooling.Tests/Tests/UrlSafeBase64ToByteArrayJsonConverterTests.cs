using System.Text;
using System.Text.Json;
using Backbone.Tooling.JsonConverters;

namespace Backbone.Tooling.Tests.Tests;

public class UrlSafeBase64ToByteArrayJsonConverterTests : AbstractTestsBase
{
    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new() { Converters = { new UrlSafeBase64ToByteArrayJsonConverter() } };

    [Fact]
    public void ShouldDeserializeJsonCorrectly()
    {
        const string jsonString = "{\"Bytes\":\"LS0tKysjIyM8PDw-Pi0oKS8iKSQlJiY_IQ\"}";

        var result = JsonSerializer.Deserialize<Test>(jsonString, JSON_SERIALIZER_OPTIONS)!;

        var utf8 = Encoding.UTF8.GetString(result.Bytes);

        utf8.ShouldBeEquivalentTo("---++###<<<>>-()/\")$%&&?!");
    }

    [Fact]
    public void ShouldSerializeJsonCorrectly()
    {
        const string expected = "{\"Bytes\":\"LS0tKysjIyM8PDw-Pi0oKS8iKSQlJiY_IQ\"}";

        var obj = new Test
        {
            Bytes = "---++###<<<>>-()/\")$%&&?!"u8.ToArray()
        };

        var serResult = JsonSerializer.Serialize(obj, JSON_SERIALIZER_OPTIONS);

        serResult.ShouldBeEquivalentTo(expected);
    }
}

public class Test
{
    public required byte[] Bytes { get; set; }
}
