using Backbone.AdminApi.Sdk.Endpoints.Messages.Types;
using Backbone.AdminApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions : BaseStepDefinitions
{
    private string _identityAddress;
    private ApiResponse<ListMessagesResponse>? _messagesResponse;

    public MessagesStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
        _identityAddress = string.Empty;
    }

    [Given(@"an Identity i")]
    public async Task GivenAnIdentity()
    {
        await CreateIdentity();
    }

    [When(@"a GET request is sent to the /Messages endpoint with type '(.*)' and participant i.Address")]
    public async Task WhenAGetRequestIsSentToTheMessagesEndpoint(string type)
    {
        _messagesResponse = await _client.Messages.GetMessagesWithParticipant(_identityAddress, new MessageType(type));
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_messagesResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response contains a paginated list of Messages")]
    public async Task ThenTheResponseContainsAListOfMessages()
    {
        _messagesResponse!.Result.ShouldNotBeNull();
        _messagesResponse!.ShouldBeASuccess();
        _messagesResponse!.ContentType.ShouldStartWith("application/json");
        await _messagesResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_messagesResponse != null)
        {
            _messagesResponse!.Error.ShouldNotBeNull();
            _messagesResponse.Error!.Code.ShouldBe(errorCode);
        }
    }

    private async Task CreateIdentity()
    {
        var createIdentityResponse = await IdentityCreationHelper.CreateIdentity(_client);
        createIdentityResponse.ShouldBeASuccess();

        _identityAddress = createIdentityResponse.Result!.Address;

        // allow the event queue to trigger the creation of this Identity on the Quotas module
        Thread.Sleep(2000);
    }
}
