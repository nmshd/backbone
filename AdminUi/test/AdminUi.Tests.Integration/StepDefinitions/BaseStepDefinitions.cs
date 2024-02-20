using Backbone.AdminUi.Tests.Integration.Models;

namespace Backbone.AdminUi.Tests.Integration.StepDefinitions;

[Binding]
public class BaseStepDefinitions
{
    protected readonly RequestConfiguration _requestConfiguration;

    public BaseStepDefinitions()
    {
        _requestConfiguration = new RequestConfiguration();
    }

    [Given("the user is authenticated")]
    public void GivenTheUserIsAuthenticated()
    {
        _requestConfiguration.Authenticate = true;
    }

    [Given("the user is unauthenticated")]
    public void GivenTheUserIsUnauthenticated()
    {
        _requestConfiguration.Authenticate = false;
    }

    [Given("the Accept header is '([^']*)'")]
    public void GivenTheAcceptHeaderIs(string acceptHeader)
    {
        _requestConfiguration.AcceptHeader = acceptHeader;
    }
}
