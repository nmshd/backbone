using Backbone.ConsumerApi.Tests.Integration.Models;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class IdentitiesApi : BaseApi
{
    public IdentitiesApi(HttpClientFactory factory) : base(factory) { }

    internal async Task<HttpResponse<CreateIdentityResponse>> CreateIdentity(RequestConfiguration requestConfiguration)
    {
        return await Post<CreateIdentityResponse>("/Identities", requestConfiguration);
    }
}
