using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
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

    private IResponse? WhenResponse => _responseContext.WhenResponse;

    #endregion

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ThrowIfNull(WhenResponse);
        ((int)WhenResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentContainsAnErrorWithTheErrorCode(string errorCode)
    {
        ThrowIfNull(WhenResponse);
        WhenResponse.Should().BeAnError();
        WhenResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then(@"the response contains a ([a-zA-Z]+)")]
    public async Task ThenTheResponseContainsA(string responseType)
    {
        ThrowIfNull(WhenResponse);
        WhenResponse.Should().BeASuccess();
        await WhenResponse!.Should().ComplyWithSchema();
    }
}
