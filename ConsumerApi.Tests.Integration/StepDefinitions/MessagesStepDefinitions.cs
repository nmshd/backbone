using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Message")]
internal class MessagesStepDefinitions
{
    private Client _sdk1 = null!;
    private Client _sdk2 = null!;
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
        _sdk1 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
        _sdk2 = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);

        var createRelationshipTemplateRequest = new CreateRelationshipTemplateRequest
        {
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };

        var relationshipTemplateResponse = await _sdk1.RelationshipTemplates.CreateTemplate(createRelationshipTemplateRequest);
        relationshipTemplateResponse.Should().BeASuccess();

        var createRelationshipRequest = new CreateRelationshipRequest
        {
            RelationshipTemplateId = relationshipTemplateResponse.Result!.Id,
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };

        var createRelationshipResponse = await _sdk2.Relationships.CreateRelationship(createRelationshipRequest);
        createRelationshipResponse.Should().BeASuccess();

        var completeRelationshipChangeRequest = new CompleteRelationshipChangeRequest
        {
            Content = ConvertibleString.FromUtf8("AAA").BytesRepresentation
        };
        var acceptRelationChangeResponse =
            await _sdk1.Relationships.AcceptChange(createRelationshipResponse.Result!.Id, createRelationshipResponse.Result.Changes.First().Id, completeRelationshipChangeRequest);
        acceptRelationChangeResponse.Should().BeASuccess();
    }

    [Given("Identity i2 is to be deleted")]
    public async Task GivenIdentityI2IsToBeDeleted()
    {
        var startDeletionProcessResponse = await _sdk2.Identities.StartDeletionProcess();
        startDeletionProcessResponse.Should().BeASuccess();
    }

    [When("a POST request is sent to the /Messages endpoint")]
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
                    Address = _sdk2.IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        };
        _sendMessageResponse = await _sdk1.Messages.SendMessage(sendMessageRequest);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_sendMessageResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains a CreateMessageResponse")]
    public void ThenTheResponseContainsADeletionProcess()
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

    [Then(@"the error contains a list of identities to be deleted that includes Identity i2")]
    public void ThenTheErrorContainsAListOfIdentitiesToBeDeletedThatIncludesIdentityI2()
    {
        var peersToBeDeleted = ((JsonElement)_sendMessageResponse!.Error!.Data!).Deserialize<List<string>>();
        peersToBeDeleted!.Contains(_sdk2.IdentityData!.Address).Should().BeTrue();
    }
}
