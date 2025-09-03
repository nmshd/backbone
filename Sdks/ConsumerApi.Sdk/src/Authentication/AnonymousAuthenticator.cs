using Backbone.BuildingBlocks.SDK.Endpoints.Common;

namespace Backbone.ConsumerApi.Sdk.Authentication;

public class AnonymousAuthenticator : IAuthenticator
{
    public Task Authenticate(HttpRequestMessage request)
    {
        throw new Exception("In order to use an authenticated request, you have to provide user credentials.");
    }
}
