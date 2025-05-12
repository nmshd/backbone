using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class ResponseStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ResponseContext _responseContext;

    public ResponseStepDefinitions(ResponseContext responseContext)
    {
        _responseContext = responseContext;
    }

    #endregion

    [Then(@"^the response status code is (\d\d\d) \(.+\)$")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ThrowIfNull(_responseContext.WhenResponse);
        ((int)_responseContext.WhenResponse.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentContainsAnErrorWithTheErrorCode(string errorCode)
    {
        ThrowIfNull(_responseContext.WhenResponse);
        _responseContext.WhenResponse.Should().BeAnError();
        _responseContext.WhenResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the response contains a ([a-zA-Z]+)")]
    public async Task ThenTheResponseContainsA(string responseType)
    {
        ThrowIfNull(_responseContext.WhenResponse);
        _responseContext.WhenResponse.Should().BeASuccess();
        await _responseContext.WhenResponse!.Should().ComplyWithSchema();
    }
}
