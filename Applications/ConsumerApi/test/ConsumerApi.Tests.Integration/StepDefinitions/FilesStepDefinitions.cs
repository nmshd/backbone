using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public FilesStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Files endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheFilesEndpoint(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);

        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("content"u8.ToArray()),
            Owner = identity.IdentityData!.Address,
            OwnerSignature = SOME_BASE64_STRING,
            CipherHash = SOME_BASE64_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = SOME_BASE64_STRING
        };

        _responseContext.WhenResponse = _responseContext.FileUploadResponse = await identity.Files.UploadFile(createFileRequest);
    }
}
