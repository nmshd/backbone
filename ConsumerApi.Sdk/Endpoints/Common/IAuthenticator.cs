namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

public interface IAuthenticator
{
    public Task Authenticate(HttpRequestMessage request);
}
