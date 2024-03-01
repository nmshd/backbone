using Backbone.AdminApi.Tests.Integration.API;
using Backbone.AdminApi.Tests.Integration.Models;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST Log")]
internal class LogsStepDefinitions : BaseStepDefinitions
{
    private readonly LogsApi _logsApi;
    private HttpResponse? _postResponse;

    public LogsStepDefinitions(LogsApi logsApi)
    {
        _logsApi = logsApi;
    }

    [When("a POST request is sent to the /Logs endpoint")]
    public async Task WhenAPOSTRequestIsSentToTheLogsEndpoint()
    {
        var createTierRequest = new LogRequest
        {
            LogLevel = LogLevel.Trace,
            Category = "Test Category",
            MessageTemplate = "The log request {0} has the following description: {1}",
            Arguments = ["Request Name", "Request Description"]
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        _postResponse = await _logsApi.CreateLog(requestConfiguration);
    }

    [When("a POST request is sent to the /Logs endpoint with an invalid Log Level")]
    public async Task WhenAPOSTRequestIsSentToTheLogsEndpointWithAnInvalidLogLevel()
    {
        var createTierRequest = new LogRequest
        {
            LogLevel = (LogLevel)16,
            Category = "Test Category",
            MessageTemplate = "The log request {0} has the following description: {1}",
            Arguments = ["Request Name", "Request Description"]
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";
        requestConfiguration.SetContent(createTierRequest);

        _postResponse = await _logsApi.CreateLog(requestConfiguration);
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_postResponse != null)
        {
            var actualStatusCode = (int)_postResponse!.StatusCode;
            actualStatusCode.Should().Be(expectedStatusCode);
        }
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _postResponse!.Content.Should().NotBeNull();
        _postResponse!.Content!.Error.Should().NotBeNull();
        _postResponse.Content.Error.Code.Should().Be(errorCode);
    }
}
