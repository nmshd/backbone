using System.Text;
using System.Text.Json;
using Enmeshed.Tooling.JsonConverters;
using FluentAssertions;
using Xunit;

namespace Enmeshed.Tooling.Tests.Tests
{
    public class UrlSafeBase64ToByteArrayJsonConverterTests
    {
        [Fact]
        public void ShouldDeserializeJsonCorrectly()
        {
            var jsonString = "{\"Bytes\":\"LS0tKysjIyM8PDw-Pi0oKS8iKSQlJiY_IQ\"}";

            var result = JsonSerializer.Deserialize<Test>(jsonString,
                new JsonSerializerOptions {Converters = {new UrlSafeBase64ToByteArrayJsonConverter()}});

            var utf8 = Encoding.UTF8.GetString(result.Bytes);

            utf8.Should().BeEquivalentTo("---++###<<<>>-()/\")$%&&?!");
        }

        [Fact]
        public void ShouldSerializeJsonCorrectly()
        {
            var expected = "{\"Bytes\":\"LS0tKysjIyM8PDw-Pi0oKS8iKSQlJiY_IQ\"}";

            var obj = new Test
            {
                Bytes = new byte[]
                {
                    45, 45, 45, 43, 43, 35, 35, 35, 60, 60, 60, 62, 62, 45, 40, 41, 47, 34, 41, 36, 37, 38, 38, 63, 33
                }
            };

            var serResult = JsonSerializer.Serialize(obj,
                new JsonSerializerOptions {Converters = {new UrlSafeBase64ToByteArrayJsonConverter()}});

            serResult.Should().BeEquivalentTo(expected);
        }
    }

    public class Test
    {
        public byte[] Bytes { get; set; }
    }
}