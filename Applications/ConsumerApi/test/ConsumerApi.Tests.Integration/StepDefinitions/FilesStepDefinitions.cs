using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Controllers.Files.DTOs;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Modules.Files.Domain.Entities;
using CSharpFunctionalExtensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{

    private readonly ResponseContext _responseContext;
    private readonly FilesContext _filesContext;

    private readonly OwnershipTokensContext _fileOwnershipTokensContext;
    private readonly ClientPool _clientPool;
    private ApiResponse<string> _resetOwnershipTokenResponse = null!;
    private ApiResponse<bool> _validateOwnershipTokenResponse = null!;
    private ApiResponse<string> _claimFileOwnershipResponse = null!;

    public FilesStepDefinitions(ResponseContext responseContext, FilesContext filesContext, ClientPool clientPool, OwnershipTokensContext fileOwnershipTokensContext)
    {
        _responseContext = responseContext;
        _filesContext = filesContext;
        _clientPool = clientPool;
        _fileOwnershipTokensContext = fileOwnershipTokensContext;
    }

    #region Given

    [Given($"File {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenFileCreatedByI(string fileName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var createResult = await Utils.CreateFile(client);
        var metadata = await client.Files.GetFileMetadata(createResult.Id);
        _filesContext.Files[fileName] = metadata.Result!;
        _fileOwnershipTokensContext.OwnershipTokens[fileName] = createResult.OwnershipToken;
    }

    [Given($"{RegexFor.SINGLE_THING} tries to claim {RegexFor.SINGLE_THING} with a wrong token")]
    public void GivenITriesToClaimFWithAWrongToken(string userName, string fileName)
    {
        var client = _clientPool.FirstForIdentityName(userName);
        var file = _filesContext.Files[fileName];
        var request = new ClaimFileOwnershipRequest
        {
            { "fileOwnershipToken", "wrongTokenXXXXXXXXXX" }
        };
        _responseContext.WhenResponse = client.Files.ClaimFileOwnership(file.Id, request).Result;
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
    public async Task WhenISendsAPatchRequestToTheFilesFIdClaimFileOwnershipFOwnershipTokenEndpoint(string identityName, string fileName, string fileName2)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];
        var token = _fileOwnershipTokensContext.OwnershipTokens[fileName];
        var request = new ClaimFileOwnershipRequest
        {
            { "fileOwnershipToken", token }
        };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(file.Id, request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/RegenerateOwnershipToken endpoint")]
    public void WhenISendsAPatchRequestToTheFilesFIdRegenerateOwnershipTokenEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id;
        fileId ??= fileName;

        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken(fileId).Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a Post request to the /Files/{RegexFor.SINGLE_THING}.Id/ValidateOwnershipToken/{RegexFor.SINGLE_THING} endpoint")]
    public void WhenISendsAPostRequestToTheFilesIllegalFileIdValidateOwnershipTokenEndpoint(string identityName, string fileName, string arbitraryToken)
    {
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id;
        fileId ??= fileName;

        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = identity.Files.ValidateFileOwnershipToken(fileId, arbitraryToken).Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/FILNonExistingXXXXXX/RegenerateOwnershipToken endpoint")]
    public void WhenISendsApatchRequestToTheFilesNonExistingRegenerateOwnershipTokenEndpoint(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = identity.Files.RegenerateFileOwnershipToken("FILNonExistingXXXXXX").Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/NonConforming/RegenerateOwnershipToken endpoint")]
    public void WhenISendsApatchRequestToTheFilesNonConformingRegenerateOwnershipTokenEndpoint(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = identity.Files.RegenerateFileOwnershipToken("NonConforming").Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a Post request to the /Files/{RegexFor.SINGLE_THING}.id/ValidateOwnershipToken/{RegexFor.SINGLE_THING}.ownershipToken endpoint")]
    public async Task WhenISendsAPostRequestToTheFilesFIdValidateOwnershipTokenFOwnershipTokenEndpoint(string identityName, string fileName, string fileNameTwo)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];
        var token = _fileOwnershipTokensContext.OwnershipTokens[fileName];
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(file.Id, token);
    }
    
    #endregion

    #region Then

    [Then("the response contains a new OwnershipToken")]
    public void ThenTheResponseContainsANewOwnershipToken()
    {
        Assert.NotNull(_claimFileOwnershipResponse);
        Assert.True(FileOwnershipToken.IsValid(_claimFileOwnershipResponse.Result!));
    }

    [Then($"the ValidateOwnershipTokenResponse is (true|false)")]
    public void ThenTheValidateOwnershipTokenResponseIsTrue(string expected)
    {
        Assert.NotNull(_validateOwnershipTokenResponse);
        Assert.Equal(_validateOwnershipTokenResponse.Result, bool.Parse(expected));
    }

    [Then($"{RegexFor.SINGLE_THING} is the new owner of {RegexFor.SINGLE_THING}")]
    public async Task ThenIIsTheNewOwnerOfF(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        //the file needs to be reloaded to get the latest owner
        var file = _filesContext.Files[fileName];
        var reloadedFile = await identity.Files.GetFileMetadata(file.Id);
        _filesContext.Files[fileName] = reloadedFile.Result!;

        identity.IdentityData.Should().NotBeNull();
        identity.IdentityData!.Address.Should().NotBeNullOrEmpty();
        identity.IdentityData!.Address.Should().Be(_filesContext.Files[fileName].Owner);
    }
    #endregion
}
