using Backbone.ConsumerApi.Sdk;
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

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    [When("(.+) sends a POST request to the /Files endpoint")]
    public async Task WhenAPostRequestIsSentToTheFilesEndpoint(string identityName)
    {
        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("content"u8.ToArray()),
            Owner = Identity(identityName).IdentityData!.Address,
            OwnerSignature = SOME_BASE64_STRING,
            CipherHash = SOME_BASE64_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = SOME_BASE64_STRING
        };

        _responseContext.WhenResponse = _responseContext.FileUploadResponse = await Identity(identityName).Files.UploadFile(createFileRequest);
    }
}
