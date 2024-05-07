using Backbone.AdminApi.Sdk.Services;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions : BaseStepDefinitions
{
    private string _identityAddress;
    private ApiResponse<List<MessageOverviewDTO>>? _messagesResponse;

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
        //_messagesResponse = await _client.Messages.GetMessagesWithParticipant(_identityAddress, type);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_messagesResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response contains a paginated list of Messages")]
    public void ThenTheResponseContainsAListOfMessages()
    {
        _messagesResponse!.Result.Should().NotBeNull();
        _messagesResponse!.IsSuccess.Should().BeTrue();
        _messagesResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_messagesResponse != null)
        {
            _messagesResponse!.Error.Should().NotBeNull();
            _messagesResponse.Error!.Code.Should().Be(errorCode);
        }
    }

    private async Task CreateIdentity()
    {
        var accountController = new AccountController(_client);
        var createIdentityResponse = await accountController.CreateIdentity(_options.ClientId, _options.ClientSecret) ?? throw new InvalidOperationException();
        createIdentityResponse.IsSuccess.Should().BeTrue();

        _identityAddress = createIdentityResponse.Result!.Address;

        // allow the event queue to trigger the creation of this Identity on the Quotas module
        Thread.Sleep(2000);
    }
}
