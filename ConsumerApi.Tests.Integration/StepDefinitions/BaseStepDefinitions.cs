using ConsumerApi.Tests.Integration.Configuration;
using ConsumerApi.Tests.Integration.Models;
using Microsoft.Extensions.Options;

namespace ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
public class BaseStepDefinitions
{
    protected readonly RequestConfiguration _requestConfiguration;

    public BaseStepDefinitions(IOptions<HttpConfiguration> httpConfiguration)
    {
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
}
