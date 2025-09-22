using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Crypto;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class MessagesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly MessagesContext _messagesContext;
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    private ApiResponse<ListMessagesResponse>? _getMessagesResponse;
    private ApiResponse<SendMessageResponse>? _sendMessageResponse;

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
        _responseContext.WhenResponse = _getMessagesResponse = await sender.Messages.ListMessages();
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Messages endpoint with {RegexFor.SINGLE_THING} as recipient")]
    public async Task WhenIdentitySendsAPostRequestToTheMessagesEndpoint(string senderIdentityName, string recipientIdentityName)
    {
        var recipientAddress = _clientPool.FirstForIdentityName(recipientIdentityName).IdentityData!.Address;

        var sendMessageRequest = new SendMessageRequest
        {
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Some Message").BytesRepresentation,
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = recipientAddress,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };

        var client = _clientPool.FirstForIdentityName(senderIdentityName);
        _responseContext.WhenResponse = _sendMessageResponse = await client.Messages.SendMessage(sendMessageRequest);
    }

    #endregion

    #region Then

    [Then($"the address of the recipient {RegexFor.SINGLE_THING} is anonymized")]
    public void ThenTheAddressOfTheRecipientIsAnonymized(string anonymizedIdentityName)
    {
        var addressOfIdentityThatShouldBeAnonymized = _clientPool.FirstForIdentityName(anonymizedIdentityName).IdentityData!.Address;

        _getMessagesResponse.ShouldNotBeNull();

        var sentMessage = _getMessagesResponse.Result!.First();

        var otherRecipients = sentMessage.Recipients.Select(r => r.Address).Where(a => a != addressOfIdentityThatShouldBeAnonymized);
        var recipientAddressesAfterGet = sentMessage.Recipients.Select(r => r.Address).ToList();

        recipientAddressesAfterGet.ShouldContain(otherRecipients);
        recipientAddressesAfterGet.ShouldContain(IdentityAddress.GetAnonymized("localhost").Value);
        recipientAddressesAfterGet.ShouldNotContain(addressOfIdentityThatShouldBeAnonymized);
    }

    [Then(@"the error contains a list of Identities to be deleted that includes ([a-zA-Z0-9]+)")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentity(string identityName)
    {
        var errorData = _sendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        errorData.ShouldNotBeNull();
        errorData.PeersToBeDeleted.Contains(_clientPool.FirstForIdentityName(identityName).IdentityData!.Address).ShouldBeTrue();
    }

    [Then(@"the response contains the Messages ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessages(string message1Name, string message2Name)
    {
        var message1 = _messagesContext.Messages[message1Name];
        var message2 = _messagesContext.Messages[message2Name];

        _getMessagesResponse.ShouldNotBeNull();

        _getMessagesResponse.Result!.ShouldContain(m => m.Id == message1.Id);
        _getMessagesResponse.Result!.ShouldContain(m => m.Id == message2.Id);
    }

    [Then(@"the response contains the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessage(string messageName)
    {
        var message = _messagesContext.Messages[messageName];

        _getMessagesResponse.ShouldNotBeNull();

        _getMessagesResponse.Result!.ShouldContain(m => m.Id == message.Id);
    }

    [Then(@"the response does not contain the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseDoesNotContainTheMessage(string messageName)
    {
        var message = _messagesContext.Messages[messageName];

        _getMessagesResponse.ShouldNotBeNull();

        _getMessagesResponse.Result!.ShouldNotContain(m => m.Id == message.Id);
    }

    #endregion
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
