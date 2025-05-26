using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Application.Tests.TestHelpers;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Application.Tests.Tests.Messages.DTOs;

public class MessageDTOTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";
    private static readonly IdentityAddress ANONYMIZED_ADDRESS = IdentityAddress.GetAnonymized(DID_DOMAIN_NAME);

    [Fact]
    public void Recipients_only_see_themselves_in_the_list_of_recipients()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();

        // Act
        var messageDTO = new MessageDTO(message, message.Recipients.First().Address, DID_DOMAIN_NAME);

        // Assert
        messageDTO.Recipients.ShouldHaveCount(1);
        messageDTO.Recipients.First().Address.ShouldBe(message.Recipients.First().Address);
    }

    [Fact]
    public void Sender_sees_all_recipients()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();

        // Act
        var messageDTO = new MessageDTO(message, message.CreatedBy, DID_DOMAIN_NAME);

        // Assert
        messageDTO.Recipients.ShouldHaveCount(2);
        messageDTO.Recipients.First().Address.ShouldBe(message.Recipients.First().Address);
        messageDTO.Recipients.Second().Address.ShouldBe(message.Recipients.Second().Address);
    }

    [Fact]
    public void A_sender_who_has_decomposed_the_relationship_to_a_recipient_sees_the_recipient_as_anonymized()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, ANONYMIZED_ADDRESS);

        // Act
        var messageDTO = new MessageDTO(message, message.CreatedBy, DID_DOMAIN_NAME);

        // Assert
        messageDTO.Recipients.First().Address.ShouldBe(ANONYMIZED_ADDRESS);
    }
}
