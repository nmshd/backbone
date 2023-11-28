using Backbone.ConsumerApi.Tests.Integration.Models;

namespace Backbone.ConsumerApi.Tests.Integration.API;

internal class IdentitiesApi : BaseApi
{
    public IdentitiesApi(HttpClientFactory factory) : base(factory) { }

    internal async Task<HttpResponse<StartDeletionProcessResponse>> StartDeletionProcess(RequestConfiguration requestConfiguration)
    {
        return await Post<StartDeletionProcessResponse>("/Identities/Self/DeletionProcesses", requestConfiguration);
    }

    internal async Task<HttpResponse<StartDeletionProcessAsSupportResponse>> StartDeletionProcessAsSupport(string id, RequestConfiguration requestConfiguration)
    {
        return await Post<StartDeletionProcessAsSupportResponse>($"/Identities/{id}/DeletionProcesses", requestConfiguration);
    }
}
