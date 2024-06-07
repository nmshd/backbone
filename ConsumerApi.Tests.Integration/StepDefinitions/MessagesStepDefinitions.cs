using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Message")]
internal class MessagesStepDefinitions
{
    private Client _client1 = null!;
    private Client _client2 = null!;
    private ApiResponse<SendMessageResponse>? _sendMessageResponse;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    public MessagesStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
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
        _sendMessageResponse = await _client1.Messages.SendMessage(sendMessageRequest);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_sendMessageResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains a SendMessageResponse")]
    public void ThenTheResponseContainsASendMessageResponse()
    {
        _sendMessageResponse!.Result.Should().NotBeNull();
        _sendMessageResponse.Should().BeASuccess();
        _sendMessageResponse.Should().ComplyWithSchema();
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
        var data = ((JsonElement)_sendMessageResponse!.Error!.Data!).Deserialize<SendMessageErrorData>(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        data!.PeersToBeDeleted.Contains(_client2.IdentityData!.Address).Should().BeTrue();
    }
}

public class SendMessageErrorData
{
    public List<string> PeersToBeDeleted { get; set; } = [];
}
