using Backbone.AdminApi.Sdk;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET /docs/index.html")]
[Scope(Feature = "GET /docs/v1/openapi.json")]
internal class DocsStepDefinitions
{
    private readonly Client _client;
    private ApiResponse<string>? _whenResponse;

    public DocsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options)
    {
        _client = Client.Create(factory.CreateClient(), options.Value.ApiKey);
    }

    [When("^a GET request is sent to the /docs/index.html endpoint$")]
    public async Task WhenAGETRequestIsSentToTheDocsIndexHtmlEndpoint()
    {
        _whenResponse = await _client.Docs.GetSwaggerUi();
    }

    [When("^a GET request is sent to the /docs/v1/openapi.json endpoint$")]
    public async Task WhenAGETRequestIsSentToTheDocsV1OpenApiJsonEndpoint()
    {
        _whenResponse = await _client.Docs.GetOpenApiSpecV1();
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();

        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }
}
