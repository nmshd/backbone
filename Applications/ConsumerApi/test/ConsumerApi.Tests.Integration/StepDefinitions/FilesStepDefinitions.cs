using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly FilesContext _filesContext;

    private readonly ClientPool _clientPool;
    private ApiResponse<RegenerateFileOwnershipResponse>? _resetOwnershipTokenResponse;
    private ApiResponse<ValidateFileOwnershipTokenResponse> _validateOwnershipTokenResponse = null!;
    private ApiResponse<ClaimFileOwnershipResponse>? _claimFileOwnershipResponse;

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
        var createResult = await Utils.CreateFile(client);
        var metadata = await client.Files.GetFileMetadata(createResult.Id);
        _filesContext.Files[fileName] = metadata.Result!;
        _filesContext.FileNameToOwnershipToken[fileName] = createResult.OwnershipToken;
    }

    [Given($"the ownership of {RegexFor.SINGLE_THING} is locked by {RegexFor.SINGLE_THING}")]
    public async Task TheOwnershipOfFIsLocked(string fileName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];
        var request = new ClaimFileOwnershipRequest { OwnershipToken = "wrongTokenXXXXXXXXXX" };
        _responseContext.WhenResponse = await client.Files.ClaimFileOwnership(file.Id, request);
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

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimOwnership with the file's ownership token")]
    public async Task WhenISendsAPatchRequestToTheFilesFIdClaimOwnershipWithTheOwnershipTokenFOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);

        var fileId = _filesContext.Files[fileName].Id;
        var token = _filesContext.FileNameToOwnershipToken[fileName];
        var request = new ClaimFileOwnershipRequest { OwnershipToken = token };

        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(fileId, request);

        if (!_claimFileOwnershipResponse.IsError)
            _filesContext.FileNameToOwnershipToken[fileName] = _claimFileOwnershipResponse.Result!.NewOwnershipToken;
        var getFileResponse = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = getFileResponse.Result!;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimOwnership with an incorrect ownership token")]
    public async Task WhenISendsApatchRequestToTheFilesFIdClaimOwnershipWithAnIncorrectOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        var request = new ClaimFileOwnershipRequest { OwnershipToken = FileOwnershipToken.New().Value };

        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(fileId, request);

        var getFileResponse = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = getFileResponse.Result!;
    }


    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimOwnership with an invalid ownership token")]
    public async Task WhenISendsApatchRequestToTheFilesFIdClaimOwnershipWithAMalformedToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;

        var request = new ClaimFileOwnershipRequest { OwnershipToken = "ownershipTokenWithMoreThan20Characters" };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(fileId, request);
    }

    [When($"{RegexFor.SINGLE_THING} tries to claim a file with an invalid FileId")]
    public async Task WhenITriesToClaimAFileWithAInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ClaimFileOwnershipRequest { OwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership("InvalidFileId", request);
    }

    [When($"{RegexFor.SINGLE_THING} tries to claim a non-existing file")]
    public async Task WhenITriesToClaimANonExistingFile(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ClaimFileOwnershipRequest { OwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(FileId.New(), request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/RegenerateOwnershipToken endpoint")]
    public async Task WhenISendsAPatchRequestToTheFilesFIdRegenerateOwnershipTokenEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = await identity.Files.RegenerateFileOwnershipToken(fileId);

        if (_resetOwnershipTokenResponse.Result != null)
            _filesContext.FileNameToOwnershipToken[fileName] = _resetOwnershipTokenResponse.Result!.NewOwnershipToken ?? "";
    }

    [When($"{RegexFor.SINGLE_THING} tries to regenerate the OwnershipToken for an invalid FileId")]
    public async Task WhenITriesToRegenerateTheOwnershiptokenForAnInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = await identity.Files.RegenerateFileOwnershipToken("InvalidFileId");
    }

    [When($"{RegexFor.SINGLE_THING} tries to regenerate the OwnershipToken of a non-existing FileId")]
    public async Task WhenITriesToRegenerateTheOwnershipTokenOfANonExistingFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = await identity.Files.RegenerateFileOwnershipToken(FileId.New());
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Files/{RegexFor.SINGLE_THING}.Id/ValidateOwnershipToken with the file's ownership token")]
    public async Task WhenISendsAPostRequestToTheFilesFIdValidateOwnershipTokenWithTheFilesOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        var token = _filesContext.FileNameToOwnershipToken[fileName];
        var request = new ValidateFileOwnershipTokenRequest { OwnershipToken = token };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(fileId, request);
    }

    [When($"{RegexFor.SINGLE_THING} tries to validate an OwnershipToken of an invalid FileId")]
    public async Task WhenITriesToValidateTheOwnershipTokenOfAnInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ValidateFileOwnershipTokenRequest { OwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken("InvalidFileId", request);
    }

    [When($"{RegexFor.SINGLE_THING} tries to validate the token using an valid FileId of a non-existing file")]
    public async Task WhenITriesToValidateTheTokenUsingAnValidFileIdOfANonExistingFile(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ValidateFileOwnershipTokenRequest { OwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(FileId.New(), request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Files/{RegexFor.SINGLE_THING}.Id/ValidateOwnershipToken with a wrong ownership token")]
    public async Task WhenISendsAPostRequestToTheFilesFIdValidateOwnershipTokenWithAWrongOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        var request = new ValidateFileOwnershipTokenRequest { OwnershipToken = "wrongTokenXXXXXXXXXX" };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(fileId, request);

        var getFileResponse = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = getFileResponse.Result!;
    }

    #endregion

    #region Then

    [Then($"the response contains the new OwnershipToken of {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseContainsTheNewOwnershipTokenOfF(string fileName)
    {
        string responseToken;
        if (_claimFileOwnershipResponse != null)
            responseToken = _claimFileOwnershipResponse.Result!.NewOwnershipToken;
        else if (_resetOwnershipTokenResponse != null)
            responseToken = _resetOwnershipTokenResponse.Result!.NewOwnershipToken;
        else
            throw new Exception("No response available");

        _filesContext.FileNameToOwnershipToken[fileName].ShouldBeEquivalentTo(responseToken);
    }


    [Then($"{RegexFor.SINGLE_THING} is the new owner of {RegexFor.SINGLE_THING}")]
    public async Task ThenIIsTheNewOwnerOfF(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        // The file needs to be reloaded to get the latest owner
        var file = _filesContext.Files[fileName];
        var reloadedFile = await identity.Files.GetFileMetadata(file.Id);
        _filesContext.Files[fileName] = reloadedFile.Result!;

        identity.IdentityData.ShouldNotBeNull();
        identity.IdentityData!.Address.ShouldNotBeNullOrEmpty();
        identity.IdentityData!.Address.ShouldBe(_filesContext.Files[fileName].Owner);
    }

    [Then($"the ownership of {RegexFor.SINGLE_THING} is locked")]
    public void ThenTheOwnershipOfFIsLocked(string fileName)
    {
        var file = _filesContext.Files[fileName];
        file.ShouldNotBeNull();
        file.OwnershipIsLocked.ShouldBeTrue();
    }

    [Then($"the ownership of {RegexFor.SINGLE_THING} is not locked")]
    public void ThenTheOwnershipOfFIsNotLocked(string fileName)
    {
        var file = _filesContext.Files[fileName];
        file.ShouldNotBeNull();
        file!.OwnershipIsLocked.ShouldBeFalse();
    }

    [Then($"the ValidateOwnershipTokenResponse contains {RegexFor.SINGLE_THING}")]
    public void ThenTheValidateOwnershipTokenResponseContainsExpected(string expected)
    {
        _validateOwnershipTokenResponse.ShouldNotBeNull();
        _validateOwnershipTokenResponse.Result.ShouldNotBeNull();
        _validateOwnershipTokenResponse.Result!.IsValid.ShouldBe(expected == "true");
    }

    #endregion
}
