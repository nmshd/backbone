using Backbone.AdminApi.Sdk.Endpoints.Logs.Types.Requests;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.SdkStepDefinitions;

[Binding]
[Scope(Feature = "POST Log")]
internal class LogsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<EmptyResponse>? _postResponse;

    public LogsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
    }

    [When("a POST request is sent to the /Logs endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheLogsEndpoint()
    {
        _postResponse = await _client.Logs.CreateLog(new LogRequest
        {
            LogLevel = LogLevel.Trace,
            Category = "Test Category",
            MessageTemplate = "The log request {0} has the following description: {1}",
            Arguments = ["Request Name", "Request Description"]
        });
    }

    [When("a POST request is sent to the /Logs endpoint with an invalid Log Level")]
    public async Task WhenAPOSTRequestIsSentToTheLogsEndpointWithAnInvalidLogLevel()
    {
        _postResponse = await _client.Logs.CreateLog(new LogRequest
        {
            LogLevel = (LogLevel)16,
            Category = "Test Category",
            MessageTemplate = "The log request {0} has the following description: {1}",
            Arguments = ["Request Name", "Request Description"]
        });
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_postResponse != null)
            ((int)_postResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _postResponse!.Result!.Error.Should().NotBeNull();
        _postResponse!.Result!.Error!.Code.Should().Be(errorCode);
    }
}
