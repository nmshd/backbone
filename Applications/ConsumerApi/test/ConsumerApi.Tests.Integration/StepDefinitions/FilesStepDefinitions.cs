using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly FilesContext _filesContext;
    private readonly ClientPool _clientPool;
    private ApiResponse<OwnershipToken> _resetOwnershipTokenResponse = null!;

    public FilesStepDefinitions(ResponseContext responseContext, FilesContext filesContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _filesContext = filesContext;
        _clientPool = clientPool;
    }

    #region Given

    [Given($"File {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenFileCreatedByI(string fileName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);

        _filesContext.Files[fileName] = await Utils.CreateFile(client);
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Files endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheFilesEndpoint(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);

        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("content"u8.ToArray()),
            Owner = identity.IdentityData!.Address,
            OwnerSignature = TestData.SOME_BASE64_STRING,
            CipherHash = TestData.SOME_BASE64_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = TestData.SOME_BASE64_STRING
        };

        _responseContext.WhenResponse = await identity.Files.UploadFile(createFileRequest);
    }

    [When($"{RegexFor.SINGLE_THING} sends a DELETE request to the /Files/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenISendsADeleteRequestToTheFilesIdEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];

        _responseContext.WhenResponse = await identity.Files.DeleteFile(file.Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Files/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenISendsAGetRequestToTheFilesIdEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];

        _responseContext.WhenResponse = await identity.Files.GetFileMetadata(file.Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership/{RegexFor.SINGLE_THING}.OwnershipToken endpoint")]
    public async void WhenISendsAPatchRequestToTheFilesFIdClaimFileOwnershipFOwnershipTokenEndpoint(string identityName, string fileName, string fileName2)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];

        _responseContext.WhenResponse = await identity.Files.ClaimFileOwnership(file.Id, file.OwnershipToken);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/RegenerateOwnershipToken endpoint")]
    public void WhenISendsAPatchRequestToTheFilesFIdRegenerateOwnershipTokenEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken(file.Id).Result;
    }

    #endregion

    #region Then

    [Then("the response contains a new OwnershipToken")]
    public void ThenTheResponseContainsANewOwnershipToken()
    {
        Assert.NotNull(_resetOwnershipTokenResponse);
        Assert.NotNull(_resetOwnershipTokenResponse.Result);
        Assert.NotNull(_resetOwnershipTokenResponse.Result.Token);
    }

    #endregion
}
