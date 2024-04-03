namespace Backbone.BuildingBlocks.SDK.Endpoints.Common;

public interface IAuthenticator
{
    public Task Authenticate(HttpRequestMessage request);
}
