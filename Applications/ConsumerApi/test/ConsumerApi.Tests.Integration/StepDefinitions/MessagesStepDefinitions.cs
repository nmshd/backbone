using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Crypto;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class MessagesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly MessagesContext _messagesContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public MessagesStepDefinitions(MessagesContext messagesContext, ResponseContext responseContext, ClientPool clientPool)
    {
        _messagesContext = messagesContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"{RegexFor.SINGLE_THING} has sent a Message {RegexFor.SINGLE_THING} to {RegexFor.LIST_OF_THINGS}")]
    public async Task GivenIdentityHasSentMessageToIdentity(string senderName, string messageName, string recipientNames)
    {
        var sender = _clientPool.FirstForIdentityName(senderName);
        var recipients = _clientPool.GetAllForIdentityNames(SplitNames(recipientNames));

        _messagesContext.Messages[messageName] = await SendMessage(sender, recipients);
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Messages endpoint")]
    public async Task WhenIdentitySendsAGetRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = _clientPool.FirstForIdentityName(senderName);
        _responseContext.WhenResponse = _responseContext.GetMessagesResponse = await sender.Messages.ListMessages();
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Messages endpoint with {RegexFor.SINGLE_THING} as recipient")]
    public async Task WhenIdentitySendsAPostRequestToTheMessagesEndpoint(string senderIdentityName, string recipientIdentityName)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Some Message").BytesRepresentation,
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = _clientPool.FirstForIdentityName(recipientIdentityName).IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };

        var client = _clientPool.FirstForIdentityName(senderIdentityName);
        _responseContext.WhenResponse = _responseContext.SendMessageResponse = await client.Messages.SendMessage(sendMessageRequest);
    }

    #endregion

    #region Then

    [Then($"the address of the recipient {RegexFor.SINGLE_THING} is anonymized")]
    public void ThenTheAddressOfTheRecipientIsAnonymized(string anonymizedIdentityName)
    {
        var addressOfIdentityThatShouldBeAnonymized = _clientPool.FirstForIdentityName(anonymizedIdentityName).IdentityData!.Address;

        ThrowIfNull(_responseContext.GetMessagesResponse);

        var sentMessage = _responseContext.GetMessagesResponse.Result!.First();

        var otherRecipients = sentMessage.Recipients.Select(r => r.Address).Where(a => a != addressOfIdentityThatShouldBeAnonymized);
        var recipientAddressesAfterGet = sentMessage.Recipients.Select(r => r.Address).ToList();

        recipientAddressesAfterGet.Should().Contain(otherRecipients);
        recipientAddressesAfterGet.Should().Contain(IdentityAddress.GetAnonymized("localhost").Value);
        recipientAddressesAfterGet.Should().NotContain(addressOfIdentityThatShouldBeAnonymized);
    }

    #endregion
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
