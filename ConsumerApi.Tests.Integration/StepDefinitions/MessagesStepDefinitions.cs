using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Options;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Message")]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions
{
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    
    private readonly IdentitiesContext _identitiesContext;
    private readonly MessagesContext _messagesContext;
    private readonly ResponseContext _responseContext;

    public MessagesStepDefinitions(IdentitiesContext identitiesContext, MessagesContext messagesContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _identitiesContext = identitiesContext;
        _messagesContext = messagesContext;
        _responseContext = responseContext;
    }

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];
    private Client[] Identities(List<string> splitIdentityNames) => _identitiesContext.Identities.GetMultiple(splitIdentityNames);

    [Given(@"([a-zA-Z0-9]+) has sent a Message ([a-zA-Z0-9]+) to (.+)")]
    public async Task GivenIHasSentMessageToI(string senderName, string messageName, string recipientNames)
    {
        var splitRecipientNames = SplitNames(recipientNames);

        var sender = Identity(senderName);
        var recipients = Identities(splitRecipientNames);

        _messagesContext.Messages[messageName] = await Utils.SendMessage(sender, recipients);
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Messages endpoint")]
    public async Task WhenISendsAGETRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = Identity(senderName);
        var getMessagesResponse = await sender.Messages.ListMessages();
        _responseContext.WhenResponse = _responseContext.GetMessagesResponse = getMessagesResponse;
    }

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

    //[Given("Identities i1 and i2 with an established Relationship")]
    //public async Task GivenIdentitiesI1AndI2WithAnEstablishedRelationship()
    //{
    //    _client1 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
    //    _client2 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);

    //    await Utils.EstablishRelationshipBetween(_client1, _client2);
    //}

    [When("([a-zA-Z0-9]+) sends a POST request to the /Messages endpoint with ([a-zA-Z0-9]+) as recipient")]
    public async Task WhenAPostRequestIsSentToTheMessagesEndpoint(string identity1Name, string identity2Name)
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

    private static List<string> SplitNames(string identityNames)
    {
        return identityNames.Split([", ", " and "], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
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
