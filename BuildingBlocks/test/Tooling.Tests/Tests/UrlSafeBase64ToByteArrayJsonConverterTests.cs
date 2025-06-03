using System.Text;
using System.Text.Json;
using Backbone.Tooling.JsonConverters;

namespace Backbone.Tooling.Tests.Tests;

public class UrlSafeBase64ToByteArrayJsonConverterTests : AbstractTestsBase
{
    [Fact]
    public void ShouldDeserializeJsonCorrectly()
    {
        const string jsonString = "{\"Bytes\":\"LS0tKysjIyM8PDw-Pi0oKS8iKSQlJiY_IQ\"}";

        var result = JsonSerializer.Deserialize<Test>(jsonString,
            new JsonSerializerOptions { Converters = { new UrlSafeBase64ToByteArrayJsonConverter() } })!;

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

        var serResult = JsonSerializer.Serialize(obj,
            new JsonSerializerOptions { Converters = { new UrlSafeBase64ToByteArrayJsonConverter() } });

        serResult.ShouldBeEquivalentTo(expected);
    }
}

public class Test
{
    public required byte[] Bytes { get; set; }
}
