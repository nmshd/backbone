using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Extensions;

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
        _responseContext.WhenResponse.ShouldNotBeNull();
        ((int)_responseContext.WhenResponse.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentContainsAnErrorWithTheErrorCode(string errorCode)
    {
        _responseContext.WhenResponse.ShouldNotBeNull();
        _responseContext.WhenResponse.ShouldBeAnError();
        _responseContext.WhenResponse.Error!.Code.ShouldBe(errorCode);
    }

    [Then(@"the response contains a ([a-zA-Z]+)")]
    public async Task ThenTheResponseContainsA(string responseType)
    {
        _responseContext.WhenResponse.ShouldNotBeNull();
        _responseContext.WhenResponse.ShouldBeASuccess();
        await _responseContext.WhenResponse!.ShouldComplyWithSchema();
    }
}
