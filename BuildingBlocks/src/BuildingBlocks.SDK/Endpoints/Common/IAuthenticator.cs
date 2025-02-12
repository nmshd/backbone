namespace Backbone.BuildingBlocks.SDK.Endpoints.Common;

public interface IAuthenticator
{
    Task Authenticate(HttpRequestMessage request);
}
