using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.AdminApi.Tests.Integration.TestData;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET Messages")]
internal class MessagesStepDefinitions : BaseStepDefinitions
{
    private readonly MessagesApi _messagesApi;
    private string _identityAddress;
    private HttpResponse<List<MessageOverviewDTO>>? _messagesResponse;

    public MessagesStepDefinitions(MessagesApi messagesApi)
    {
        _messagesApi = messagesApi;
        _identityAddress = string.Empty;
    }

    [Given(@"an Identity i")]
    public void GivenAnIdentity()
    {
        _identityAddress = Identities.IDENTITY_A;
    }

    [When(@"a GET request is sent to the /Messages endpoint with type '(.*)' and participant i.Address")]
    public async Task WhenAGetRequestIsSentToTheMessagesEndpoint(string type)
    {
        _messagesResponse = await _messagesApi.GetMessagesWithParticipant(_identityAddress, type, _requestConfiguration);
        _messagesResponse.Should().NotBeNull();
        _messagesResponse.Content.Should().NotBeNull();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_messagesResponse != null)
        {
            var actualStatusCode = (int)_messagesResponse.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response contains a paginated list of Messages")]
    public void ThenTheResponseContainsAListOfMessages()
    {
        _messagesResponse!.AssertHasValue();
        _messagesResponse!.AssertStatusCodeIsSuccess();
        _messagesResponse!.AssertContentTypeIs("application/json");
        _messagesResponse!.AssertContentCompliesWithSchema();
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        if (_messagesResponse != null)
        {
            _messagesResponse!.Content.Error.Should().NotBeNull();
            _messagesResponse.Content.Error!.Code.Should().Be(errorCode);
        }
    }
}
