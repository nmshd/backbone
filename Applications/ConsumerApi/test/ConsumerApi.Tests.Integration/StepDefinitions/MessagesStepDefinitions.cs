using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Crypto;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class MessagesStepDefinitions
{
    private readonly IdentitiesContext _identitiesContext;
    private readonly MessagesContext _messagesContext;
    private readonly ResponseContext _responseContext;

    public MessagesStepDefinitions(IdentitiesContext identitiesContext, MessagesContext messagesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _messagesContext = messagesContext;
        _responseContext = responseContext;
    }

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];
    private Client[] Identities(List<string> splitIdentityNames) => _identitiesContext.Identities.GetMultiple(splitIdentityNames);

    #region Given
    [Given(@"([a-zA-Z0-9]+) has sent a Message ([a-zA-Z0-9]+) to (.+)")]
    public async Task GivenIHasSentMessageToI(string senderName, string messageName, string recipientNames)
    {
        var splitRecipientNames = SplitNames(recipientNames);

        var sender = Identity(senderName);
        var recipients = Identities(splitRecipientNames);

        _messagesContext.Messages[messageName] = await Utils.SendMessage(sender, recipients);
    }
    #endregion

    #region When
    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Messages endpoint")]
    public async Task WhenISendsAGetRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = Identity(senderName);
        var getMessagesResponse = await sender.Messages.ListMessages();
        _responseContext.WhenResponse = _responseContext.GetMessagesResponse = getMessagesResponse;
    }

    [When("([a-zA-Z0-9]+) sends a POST request to the /Messages endpoint with ([a-zA-Z0-9]+) as recipient")]
    public async Task WhenISendsAPostRequestToTheMessagesEndpoint(string identity1Name, string identity2Name)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Some Message").BytesRepresentation,
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = Identity(identity2Name).IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };

        _responseContext.WhenResponse = _responseContext.SendMessageResponse = await Identity(identity1Name).Messages.SendMessage(sendMessageRequest);
    }
    #endregion

    #region Then
    [Then(@"the address of the recipient ([a-zA-Z0-9]+) is anonymized")]
    public void ThenTheAddressOfIIsAnonymized(string anonymizedIdentityName)
    {
        var addressOfIdentityThatShouldBeAnonymized = Identity(anonymizedIdentityName).IdentityData!.Address;

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

public class MessagesContext
{
    public readonly Dictionary<string, Message> Messages = new();
}

public static class DictionaryExtensions
{
    public static TValue[] GetMultiple<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, List<TKey> keys) where TKey : notnull
    {
        return dictionary.Where(x => keys.Contains(x.Key)).Select(x => x.Value).ToArray();
    }
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
