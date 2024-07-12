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
using Microsoft.Extensions.Options;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Message")]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions
{
    private Client _client1 = null!;
    private Client _client2 = null!;
    private ApiResponse<SendMessageResponse>? _sendMessageResponse;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, Client> _identities = new();
    private readonly Dictionary<string, Relationship> _relationships = new();
    private readonly Dictionary<string, Message> _messages = new();
    private ApiResponse<ListMessagesResponse>? _getMessagesResponse;
    private IResponse? _whenResponse;

    public MessagesStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given(@"Identities ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void GivenIdentitiesIAndI(string identity1Name, string identity2Name)
    {
        _identities[identity1Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
        _identities[identity2Name] = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
    }

    [Given(@"a Relationship ([a-zA-Z0-9]+) between ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public async Task GivenARelationshipRBetweenIAndI(string relationshipName, string identity1Name, string identity2Name)
    {
        var relationship = await Utils.EstablishRelationshipBetween(_identities[identity1Name], _identities[identity2Name]);
        _relationships[relationshipName] = relationship;
    }

    [Given(@"([a-zA-Z0-9]+) has sent a Message ([a-zA-Z0-9]+) to ([a-zA-Z0-9]+)")]
    public async Task GivenIHasSentMessageToI(string senderName, string messageName, string recipientName)
    {
        var sender = _identities[senderName];
        var recipient = _identities[recipientName];
        var message = await Utils.SendMessage(sender, recipient);
        _messages[messageName] = message;
    }

    [Given(@"([a-zA-Z0-9]+) has decomposed ([a-zA-Z0-9]+)")]
    public async Task GivenIHasDecomposedItsRelationshipToI(string decomposerName, string peerName)
    {
        var decomposer = _identities[decomposerName];
        var relationship = _relationships[decomposerName];

        var terminateRelationshipResponse = await decomposer.Relationships.TerminateRelationship(relationship.Id);
        terminateRelationshipResponse.Should().BeASuccess();

        var decomposeRelationshipResponse = await decomposer.Relationships.DecomposeRelationship(relationship.Id);
        decomposeRelationshipResponse.Should().BeASuccess();
    }

    [When(@"([a-zA-Z0-9]+) sends a GET request to the /Messages endpoint")]
    public async Task WhenISendsAGETRequestToTheMessagesEndpoint(string senderName)
    {
        var sender = _identities[senderName];
        var getMessagesResponse = await sender.Messages.ListMessages();
        _whenResponse = _getMessagesResponse = getMessagesResponse;
    }

    [Then(@"the response contains the Messages ([a-zA-Z0-9]+) and ([a-zA-Z0-9]+)")]
    public void ThenTheResponseContainsTheMessagesMAndM(string message1Name, string message2Name)
    {
        var message1 = _messages[message1Name];
        var message2 = _messages[message2Name];

        ThrowIfNull(_getMessagesResponse);

        _getMessagesResponse.Result.Should().Contain(m => m.Id == message1.Id);
        _getMessagesResponse.Result.Should().Contain(m => m.Id == message2.Id);
    }

    [Given("Identities i1 and i2 with an established Relationship")]
    public async Task GivenIdentitiesI1AndI2WithAnEstablishedRelationship()
    {
        _client1 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
        _client2 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);

        await Utils.EstablishRelationshipBetween(_client1, _client2);
    }

    [Given("i2 is in status \"ToBeDeleted\"")]
    public async Task GivenIdentityI2IsToBeDeleted()
    {
        var startDeletionProcessResponse = await _client2.Identities.StartDeletionProcess();
        startDeletionProcessResponse.Should().BeASuccess();
    }

    [When("i1 sends a POST request to the /Messages endpoint with i2 as recipient")]
    public async Task WhenAPostRequestIsSentToTheMessagesEndpoint()
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Attachments = [],
            Body = ConvertibleString.FromUtf8("Some Message").BytesRepresentation,
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = _client2.IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };
        _whenResponse = _sendMessageResponse = await _client1.Messages.SendMessage(sendMessageRequest);
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
        _sendMessageResponse!.Result.Should().NotBeNull();
        _sendMessageResponse.Should().BeASuccess();
        await _sendMessageResponse.Should().ComplyWithSchema();
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _sendMessageResponse!.Error.Should().NotBeNull();
        _sendMessageResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the error contains a list of Identities to be deleted that includes i2")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentityI2()
    {
        var data = _sendMessageResponse!.Error!.Data?.As<PeersToBeDeletedErrorData>();
        data.Should().NotBeNull();
        data!.PeersToBeDeleted.Contains(_client2.IdentityData!.Address).Should().BeTrue();
    }
}

public class PeersToBeDeletedErrorData
{
    public required List<string> PeersToBeDeleted { get; set; }
}
