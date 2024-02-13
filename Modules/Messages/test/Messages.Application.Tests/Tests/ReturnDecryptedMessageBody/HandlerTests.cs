using System.Threading;
using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Application.Messages.Queries.ViewMessages;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Google.Protobuf;
using Microsoft.Identity.Client;
using Xunit;
using Handler = Backbone.Modules.Messages.Application.Messages.Queries.ViewMessages.Handler;

namespace Backbone.Modules.Messages.Application.Tests.Tests.ReturnDecryptedMessageBody;

public class HandlerTests
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public HandlerTests()
    {
        _mapper = A.Fake<IMapper>();
        _messagesRepository = A.Fake<IMessagesRepository>();
        _userContext = A.Fake<IUserContext>();
    }

    [Fact]
    public async Task Handler_should_throw_null_reference_exception_when_null_is_passed()
    {
        // Arrange
        var messageQuery = new GetViewMessageQuery();
        var handler = new Handler(_mapper);

        // Act

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(messageQuery, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_should_return_message_given_MessageId()
    {
        // Arrange
        var mockMessagesRepository = A.Fake<IMessagesRepository>();
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();

        var message = CreateTestMessage(identityAddress, deviceId);

        await mockMessagesRepository.Add(message, CancellationToken.None);

        var request = CreateTestViewMessageQuery(message);

        var expectedMessage = CreateExpectedViewMessageDTO(message);

        var handler = new Handler(_userContext, _mapper, _messagesRepository);

        A.CallTo(() => _messagesRepository.Find(
                request.MessageId,
                _userContext.GetAddress(),
                CancellationToken.None, true, true))
                .Returns(message);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Body.Should().BeEquivalentTo(expectedMessage.Body);
    }

    private static ViewMessageDTO CreateExpectedViewMessageDTO(Message message)
    {
        var expectedMessage = new ViewMessageDTO
        {
            Body = "{\"@type\":\"MessageSigned\",\"message\":\"{\\\"@type\\\":\\\"MessageContentWrapper\\\",\\\"attachments\\\":[],\\\"content\\\":{\\\"content\\\":\\\"TestContent\\\"},\\\"createdAt\\\":\\\"2024-02-06T22:18:39.470Z\\\",\\\"recipients\\\":[\\\"id19khDhsE7Ak18BgMHEba7iczUdcg4Zjmew\\\"]}\",\"signatures\":[{\"recipient\":\"id19khDhsE7Ak18BgMHEba7iczUdcg4Zjmew\",\"signature\":\"{\\\"sig\\\":\\\"S8wFVW_NHwmydRIpDGgjbnm5YXICimfjBaupZO7T3firEIuu_NWogZwcgXMzC5YehXyERasueSVOrkGR0PnaDg\\\",\\\"alg\\\":1}\"}]}",
            CreatedAt = DateTime.Parse("2024-02-06T22:18:39.470Z"),
            Id = message.Id
        };
        return expectedMessage;
    }

    private static GetViewMessageQuery CreateTestViewMessageQuery(Message message)
    {
        var request = new GetViewMessageQuery
        {
            MessageId = message.Id,
            SymmetricKey = "{\"Key\":\"yS6vFC70epmYFoUAIopluLmJJUH58FVGnwI3AR1fPZ8\",\"alg\":3}"
        };
        return request;
    }

    private Message CreateTestMessage(IdentityAddress identityAddress, DeviceId deviceId)
    {
        var body = "{\"cph\":\"m8coGmZkRX05QMkSsCBXrLbEwEfiVJQtE5u11ekVLuY24L8q3wwyS_s8TuymOOMHjFa3RkEUH2LlQpXi3i9W8YuRLPdvJZbujD7GquwgJknalAMs14qb-mdY5106lnEQcJ2j0EdiRZbPRzjIcXBS5vwa8l3Srdt-W4fR0QSlZYdrWGgD8IYFdkrnMhVc44PKo9-dXQ88vyIYjnM-fI5p-u_QIRWAdfjNF5Wc29YBj45hD3g0AkRgZ8uR7_vllXXh87SpMbsAxSqZZa-jDZAaUxjKgDFXbSowrBSnr0mh0UhkcwCgYMWm2nZoF1H6XYBBH72K69_mJM_4N1b1pAByDBf1sqVzwY3we9jOENc09OOTJntn5Bep9NwPmvZc8rGFhKfxMrH0ixd2CRw2YqmcEZLgMwylUAXqCQPEFIl9Uu8T4OhJuwAx80ey6luiu17r-C2Lwtr24EGqjPUafuY2TxAT-Zjk6g_ThhddPPtel6hi2ejIjM5x-ylhRuYMadxXiTp1obcDzgq5FYkxlxPRgfC8WBOf6CPobD_kAYxDO8xR7ydWBJ4XCy6K2KEtONo066LRgpbliVAhZ3ts9fw\",\"alg\":3,\"nnc\":\"XjNcSg7d4EVusbnrPTQ52juVeB1wKPUJ\",\"@type\":\"CryptoCipher\"}"u8.ToArray();
        return new Message(identityAddress, deviceId, null, body, new List<Attachment>(), new List<RecipientInformation>());

    }
}
