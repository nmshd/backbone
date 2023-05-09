using AdminApi.Tests.Integration.Utils.Models;
using AdminApi.Tests.Integration.Utils.Support;

namespace AdminApi.Tests.Integration.Utils.StepDefinitions;

[Binding]
public class BaseStepDefinitions<T>
{
    protected readonly RequestConfiguration _requestConfiguration;
    protected HttpResponse<T> _response;

    protected BaseStepDefinitions(HttpResponse<T> response)
    {
        _response = response;
        _requestConfiguration = new RequestConfiguration();
    }

    [Given(@"the user is authenticated")]
    public void GivenTheUserIsAuthenticated()
    {
        _requestConfiguration.Authenticate = true;
    }

    [Given(@"the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.Authenticate = false;
    }

    [Given(@"the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the response content includes an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _response.Content!.Error.Should().NotBeNull();
        _response.Content!.Error!.Code.Should().Be(errorCode);
    }

    protected void AssertResponseIntegrity()
    {
        _response.Should().NotBeNull();

        AssertStatusCodeIsSuccess();
        AssertResponseContentTypeIsJson();
        AssertResponseContentCompliesWithSchema();
    }

    protected void AssertStatusCodeIsSuccess()
    {
        _response.IsSuccessStatusCode.Should().BeTrue();
    }

    protected void AssertResponseContentTypeIsJson()
    {
        _response.ContentType.Should().Be("application/json");
    }

    protected void AssertResponseContentCompliesWithSchema()
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T>>(_response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
