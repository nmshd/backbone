using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Microsoft.Extensions.Options;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "POST File")]
internal class FilesStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ClientCredentials _clientCredentials;

    private Client? _identity;
    private ApiResponse<CreateFileResponse>? _response;
    public FilesStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given(@"Identity (.+)")]
    public void GivenIdentity(string identityName)
    {
        _identity = Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD).Result;
    }

    [When(@"(.+) sends a POST request to the /Files endpoint")]
    public async Task WhenAPostRequestIsSentToTheFilesEndpoint(string identityName)
    {
        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("whatever"u8.ToArray()),
            Owner = _identity!.IdentityData!.Address,
            OwnerSignature = "AAAA",
            CipherHash = "AAAA",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = "AAAA"
        };

        _response = await _identity!.Files.UploadFile(createFileRequest);
    }

    [Then(@"the response status code is (.*) \(.*\)")]
    public void ThenTheResponseStatusCodeIsCreated(int p0)
    {
        ThrowIfNull(_response);
        ((int)_response!.Status).Should().Be(p0);
    }

    [Then(@"the response contains a CreateFileResponse")]
    public async Task ThenTheResponseContainsACreateFileResponse()
    {
        _response!.Result.Should().NotBeNull();
        _response.Should().BeASuccess();
        await _response.Should().ComplyWithSchema();
    }
}
