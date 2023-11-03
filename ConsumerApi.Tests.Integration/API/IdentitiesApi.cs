using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Modules.Devices.ConsumerApi.Controllers;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class IdentitiesApi : BaseApi
{
    public IdentitiesApi(HttpClientFactory factory) : base(factory) { }

    internal async Task<HttpResponse> StartDeletionProcess(RequestConfiguration requestConfiguration)
    {
        return await Post("/Identities/Self/DeletionProcess", requestConfiguration);
    }

    internal async Task<HttpResponse> CreateIdentity(RequestConfiguration requestConfiguration)
    {
        return await Post("/Identities", requestConfiguration);
    }
}

public class CreateIdentityRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string IdentityPublicKey { get; set; }
    public string DevicePassword { get; set; }
    public byte IdentityVersion { get; set; }
    public CreateIdentityRequestSignedChallenge SignedChallenge { get; set; }
}

public class CreateIdentityRequestSignedChallenge
{
    public string Challenge { get; set; }
    public string Signature { get; set; }
}
