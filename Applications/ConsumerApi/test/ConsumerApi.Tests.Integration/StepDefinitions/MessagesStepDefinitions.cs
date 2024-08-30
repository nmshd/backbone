using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Crypto;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Message")]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions
{
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, Client> _identities = new();
    private readonly Dictionary<string, Relationship> _relationships = new();
    private readonly Dictionary<string, Message> _messages = new();
    private ApiResponse<ListMessagesResponse>? _getMessagesResponse;
    private ApiResponse<SendMessageResponse>? _sendMessageResponse;

    public MessagesStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    #region Given

    [Given(@"Identities (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public void Given2Identities(string identity1Name, string identity2Name)
    {
        _identities[identity1Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
        _identities[identity2Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
    }

    [Given(@"Identities (i[a-zA-Z0-9]*), (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public void Given3Identities(string identity1Name, string identity2Name, string identity3Name)
    {
        _identities[identity1Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
        _identities[identity2Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
        _identities[identity3Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
    }

    [Given(@"a Relationship (r[a-zA-Z0-9]*) between (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public async Task GivenARelationshipRBetweenIAndI(string relationshipName, string identity1Name, string identity2Name)
    {
        var relationship = await Utils.EstablishRelationshipBetween(_identities[identity1Name], _identities[identity2Name]);
        _relationships[relationshipName] = relationship;
    }

    [Given(@"(i[a-zA-Z0-9]*) has sent a Message (m[a-zA-Z0-9]*) to (i[a-zA-Z0-9]*)")]
    public async Task GivenIHasSentMessageTo1Recipient(string senderName, string messageName, string recipientName)
    {
        var sender = _identities[senderName];
        var recipient = _identities[recipientName];

        _messages[messageName] = await Utils.SendMessage(sender, recipient);
    }

    [Given(@"(i[a-zA-Z0-9]*) has sent a Message (m[a-zA-Z0-9]*) to (i[a-zA-Z0-9]*) and (i[a-zA-Z0-9]*)")]
    public async Task GivenIHasSentMessageTo2Recipients(string senderName, string messageName, string recipient1Name, string recipient2Name)
    {
        var sender = _identities[senderName];
        var recipient1 = _identities[recipient1Name];
        var recipient2 = _identities[recipient2Name];

        _messages[messageName] = await Utils.SendMessage(sender, recipient1, recipient2);
    }

    [Given(@"(i[a-zA-Z0-9]*) has terminated (r[a-zA-Z0-9]*)")]
    public async Task GivenRIsTerminated(string terminatorName, string relationshipName)
    {
        var relationship = _relationships[relationshipName];
        var terminator = _identities[terminatorName];

        var terminateRelationshipResponse = await terminator.Relationships.TerminateRelationship(relationship.Id);
        terminateRelationshipResponse.Should().BeASuccess();
    }

    [Given(@"(i[a-zA-Z0-9]*) has decomposed (r[a-zA-Z0-9]*)")]
    public async Task GivenIHasDecomposedItsRelationshipToI(string decomposerName, string relationshipName)
    {
        var decomposer = _identities[decomposerName];
        var relationship = _relationships[relationshipName];

        var decomposeRelationshipResponse = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        decomposeRelationshipResponse.Should().BeASuccess();

        await Task.Delay(500);
    }

    [Given("(i[a-zA-Z0-9]*) is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityIIsToBeDeleted(string identityName)
    {
        var identity = _identities[identityName];
        var startDeletionProcessResponse = await identity.Identities.StartDeletionProcess();
        startDeletionProcessResponse.Should().BeASuccess();
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Messages endpoint")]
    public async Task WhenIdentitySendsAGetRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = _identities[senderName];
        var getMessagesResponse = await sender.Messages.ListMessages();
        _whenResponse = _getMessagesResponse = getMessagesResponse;
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Messages endpoint with {RegexFor.SINGLE_THING} as recipient")]
    public async Task WhenIdentitySendsAPostRequestToTheMessagesEndpoint(string senderIdentityName, string recipientIdentityName)
    {
        var sender = _identities[senderName];
        var recipient = _identities[recipientName];

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
        var addressOfIdentityThatShouldBeAnonymized = _identities[anonymizedIdentityName].IdentityData!.Address;

        ThrowIfNull(_getMessagesResponse);

        var sentMessage = _getMessagesResponse.Result!.First();

        var otherRecipients = sentMessage.Recipients.Select(r => r.Address).Where(a => a != addressOfIdentityThatShouldBeAnonymized);
        var recipientAddressesAfterGet = sentMessage.Recipients.Select(r => r.Address).ToList();

        recipientAddressesAfterGet.Should().Contain(otherRecipients);
        recipientAddressesAfterGet.Should().Contain(IdentityAddress.GetAnonymized("localhost").Value);
        recipientAddressesAfterGet.Should().NotContain(addressOfIdentityThatShouldBeAnonymized);
    }

    [Then(@"the error contains a list of Identities to be deleted that includes ([a-zA-Z0-9]+)")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentity(string identityName)
    {
        var errorData = _sendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        errorData.Should().NotBeNull();
        errorData!.PeersToBeDeleted.Contains(_clientPool.FirstForIdentityName(identityName).IdentityData!.Address).Should().BeTrue();
    }

    [Then(@"the response contains the Messages ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessages(string message1Name, string message2Name)
    {
        var message1 = _messagesContext.Messages[message1Name];
        var message2 = _messagesContext.Messages[message2Name];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().Contain(m => m.Id == message1.Id);
        _getMessagesResponse.Result.Should().Contain(m => m.Id == message2.Id);
    }

    [Then(@"the response contains the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessage(string messageName)
    {
        var message = _messagesContext.Messages[messageName];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().Contain(m => m.Id == message.Id);
    }

    [Then(@"the response does not contain the Message ([a-zA-Z0-9]+)")]
    public void ThenTheResponseDoesNotContainTheMessage(string messageName)
    {
        var includedIdentity = _identities[includedIdentityName];
        var data = _sendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        data.Should().NotBeNull();
        data!.PeersToBeDeleted.Contains(includedIdentity.IdentityData!.Address).Should().BeTrue();
    }

    #endregion
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
