using ConsumerApi.Tests.Integration.Configuration;
using ConsumerApi.Tests.Integration.Models;
using ConsumerApi.Tests.Integration.Support;
using Microsoft.Extensions.Options;

namespace ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
public class BaseStepDefinitions<T>
{
    protected readonly RequestConfiguration _requestConfiguration;
    protected HttpResponse<T> _response;

    public BaseStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, HttpResponse<T> response)
    {
        _response = response;
        _requestConfiguration = new RequestConfiguration
        {
            AuthenticationParameters = new AuthenticationParameters
            {
                GrantType = "password",
                ClientId = httpConfiguration.Value.ClientCredentials.ClientId,
                ClientSecret = httpConfiguration.Value.ClientCredentials.ClientSecret,
                Username = "USRa",
                Password = "a"
            }
        };
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

    public void AssertResponseIntegrity()
    {
        _response.Should().NotBeNull();

        AssertStatusCodeIsSuccess();
        AssertResponseContentTypeIsJson();
    }

    public void AssertStatusCodeIsSuccess()
    {
        _response.IsSuccessStatusCode.Should().BeTrue();
    }

    public void AssertResponseContentTypeIsJson()
    {
        _response.ContentType.Should().Be("application/json");
    }

    public void AssertResponseContentCompliesWithSchema<T1>()
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T1>>(_response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T1).FullName} schema: {string.Join(", ", errors)}");
    }
}
