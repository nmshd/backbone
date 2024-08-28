using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public FilesStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    [When("([a-zA-Z0-9]+) sends a POST request to the /Files endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheFilesEndpoint(string identityName)
    {
        var identity = _identitiesContext.ClientPool.FirstForIdentityName(identityName);

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
