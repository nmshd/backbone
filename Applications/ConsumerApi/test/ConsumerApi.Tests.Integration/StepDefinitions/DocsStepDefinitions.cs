using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
public class DocsStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public DocsStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    [When("^a GET request is sent to the /docs/index.html endpoint$")]
    public async Task WhenAGETRequestIsSentToTheDocsIndexHtmlEndpoint()
    {
        var client = _clientPool.Anonymous;

        _responseContext.WhenResponse = await client.Docs.GetSwaggerUi();
    }

    [When("^a GET request is sent to the /docs/v1/openapi.json endpoint$")]
    public async Task WhenAGETRequestIsSentToTheDocsV1OpenApiJsonEndpoint()
    {
        var client = _clientPool.Anonymous;

        _responseContext.WhenResponse = await client.Docs.GetOpenApiSpecV1();
    }

    [When("^a GET request is sent to the /docs/v2/openapi.json endpoint$")]
    public async Task WhenAGETRequestIsSentToTheDocsV2OpenApiJsonEndpoint()
    {
        var client = _clientPool.Anonymous;

        _responseContext.WhenResponse = await client.Docs.GetOpenApiSpecV2();
    }
}
