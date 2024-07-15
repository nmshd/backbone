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

    private readonly Dictionary<string, Client> _identities = new();
    private readonly Dictionary<string, Relationship> _relationships = new();
    private readonly Dictionary<string, Message> _messages = new();
    private ApiResponse<ListMessagesResponse>? _getMessagesResponse;
    private ApiResponse<SendMessageResponse>? _sendMessageResponse;
    private IResponse? _whenResponse;

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

    [When(@"(i[a-zA-Z0-9]*) sends a GET request to the /Messages endpoint")]
    public async Task WhenISendsAGETRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = _identities[senderName];
        var getMessagesResponse = await sender.Messages.ListMessages();
        _whenResponse = _getMessagesResponse = getMessagesResponse;
    }

    [When("(i[a-zA-Z0-9]*) sends a POST request to the /Messages endpoint with (i[a-zA-Z0-9]*) as recipient")]
    public async Task WhenAPostRequestIsSentToTheMessagesEndpoint(string senderName, string recipientName)
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
                    Address = recipient.IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };
        _whenResponse = _sendMessageResponse = await sender.Messages.SendMessage(sendMessageRequest);
    }

    #endregion

    #region Then

    [Then(@"the response contains the Messages (m[a-zA-Z0-9]*) and (m[a-zA-Z0-9]*)")]
    public void ThenTheResponseContainsTheMessagesMAndM(string message1Name, string message2Name)
    {
        var message1 = _messages[message1Name];
        var message2 = _messages[message2Name];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().Contain(m => m.Id == message1.Id);
        _getMessagesResponse.Result.Should().Contain(m => m.Id == message2.Id);
    }

    [Then(@"the response does not contain the Message (m[a-zA-Z0-9]*)")]
    public void ThenTheResponseDoesNotContainTheMessageM(string messageName)
    {
        var message = _messages[messageName];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().NotContain(m => m.Id == message.Id);
    }

    [Then(@"the response contains the Message (m[a-zA-Z0-9]*)")]
    public void ThenTheResponseContainsTheMessageM(string messageName)
    {
        var message = _messages[messageName];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().Contain(m => m.Id == message.Id);
    }

    [Then(@"the address of the recipient (i[a-zA-Z0-9]*) is anonymized")]
    public void ThenTheAddressOfIIsAnonymized(string anonymizedIdentityName)
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

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ThrowIfNull(_whenResponse);
        ((int)_whenResponse.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains a SendMessageResponse")]
    public async Task ThenTheResponseContainsASendMessageResponse()
    {
        _sendMessageResponse?.Should().NotBeNull();
        _sendMessageResponse?.Should().BeASuccess();
        await _sendMessageResponse!.Should().ComplyWithSchema();
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _sendMessageResponse!.Error.Should().NotBeNull();
        _sendMessageResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the error contains a list of Identities to be deleted that includes (i[a-zA-Z0-9]*)")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentityI2(string includedIdentityName)
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
