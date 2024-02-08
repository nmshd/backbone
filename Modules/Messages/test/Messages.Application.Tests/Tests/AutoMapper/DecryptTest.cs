using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Tooling.JsonConverters;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Application.Tests.Tests.AutoMapper;
public class DecryptTest
{
    [Fact]
    public void Decrypt_Encrypted_Message()
    {
        // Arrange
        var body = "{\"cph\":\"m8coGmZkRX05QMkSsCBXrLbEwEfiVJQtE5u11ekVLuY24L8q3wwyS_s8TuymOOMHjFa3RkEUH2LlQpXi3i9W8YuRLPdvJZbujD7GquwgJknalAMs14qb-mdY5106lnEQcJ2j0EdiRZbPRzjIcXBS5vwa8l3Srdt-W4fR0QSlZYdrWGgD8IYFdkrnMhVc44PKo9-dXQ88vyIYjnM-fI5p-u_QIRWAdfjNF5Wc29YBj45hD3g0AkRgZ8uR7_vllXXh87SpMbsAxSqZZa-jDZAaUxjKgDFXbSowrBSnr0mh0UhkcwCgYMWm2nZoF1H6XYBBH72K69_mJM_4N1b1pAByDBf1sqVzwY3we9jOENc09OOTJntn5Bep9NwPmvZc8rGFhKfxMrH0ixd2CRw2YqmcEZLgMwylUAXqCQPEFIl9Uu8T4OhJuwAx80ey6luiu17r-C2Lwtr24EGqjPUafuY2TxAT-Zjk6g_ThhddPPtel6hi2ejIjM5x-ylhRuYMadxXiTp1obcDzgq5FYkxlxPRgfC8WBOf6CPobD_kAYxDO8xR7ydWBJ4XCy6K2KEtONo066LRgpbliVAhZ3ts9fw\",\"alg\":3,\"nnc\":\"XjNcSg7d4EVusbnrPTQ52juVeB1wKPUJ\",\"@type\":\"CryptoCipher\"}"u8.ToArray();

        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();
        var message = new Message(identityAddress, deviceId, null, body, new List<Attachment>(), new List<RecipientInformation>());

        var expectedBody = "{\"@type\":\"MessageSigned\",\"message\":\"{\\\"@type\\\":\\\"MessageContentWrapper\\\",\\\"attachments\\\":[],\\\"content\\\":{\\\"content\\\":\\\"TestContent\\\"},\\\"createdAt\\\":\\\"2024-02-06T22:18:39.470Z\\\",\\\"recipients\\\":[\\\"id19khDhsE7Ak18BgMHEba7iczUdcg4Zjmew\\\"]}\",\"signatures\":[{\"recipient\":\"id19khDhsE7Ak18BgMHEba7iczUdcg4Zjmew\",\"signature\":\"{\\\"sig\\\":\\\"S8wFVW_NHwmydRIpDGgjbnm5YXICimfjBaupZO7T3firEIuu_NWogZwcgXMzC5YehXyERasueSVOrkGR0PnaDg\\\",\\\"alg\\\":1}\"}]}";

        var secretKey = "{\"key\":\"yS6vFC70epmYFoUAIopluLmJJUH58FVGnwI3AR1fPZ8\",\"alg\":3}";

        // Act
        var decryptedBody = message.Decrypt(secretKey);

        // Assert
        decryptedBody.Should().BeEquivalentTo(expectedBody);
    }
}
