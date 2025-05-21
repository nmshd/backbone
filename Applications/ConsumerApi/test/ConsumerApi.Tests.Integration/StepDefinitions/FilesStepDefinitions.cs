using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
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

    [Given($"{RegexFor.SINGLE_THING} tries to claim {RegexFor.SINGLE_THING} with a wrong token")]
    public void GivenITriesToClaimFWithAWrongToken(string identityName, string fileName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = "wrongTokenXXXXXXXXXX" };
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

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership with the file's ownership token")]
    public async Task WhenISendsAPatchRequestToTheFilesFIdClaimFileOwnershipWithTheOwnershipTokenFOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);

        var fileId = _filesContext.Files[fileName].Id;
        var token = _filesContext.FileNameToOwnershipToken[fileName];
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = token };

        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(fileId, request);

        if (!_claimFileOwnershipResponse.IsError)
            _filesContext.FileNameToOwnershipToken[fileName] = _claimFileOwnershipResponse.Result!.NewOwnershipToken;
        var getFileResponse = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = getFileResponse.Result!;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership with an incorrect ownership token")]
    public async Task WhenISendsApatchRequestToTheFilesFIdClaimFileOwnershipWithAnIncorrectOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = FileOwnershipToken.New().Value };

        _responseContext.WhenResponse = _claimFileOwnershipResponse = identity.Files.ClaimFileOwnership(fileId, request).Result;

        var getFileResponse = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = getFileResponse.Result!;
    }


    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership with an invalid ownership token")]
    public void WhenISendsApatchRequestToTheFilesFIdClaimFileOwnershipWithAMalformedToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;

        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = "ownershipTokenWithMoreThan20Characters" };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = identity.Files.ClaimFileOwnership(fileId, request).Result;
    }

    [When($"{RegexFor.SINGLE_THING} tries to claim a file with an invalid FileId")]
    public void WhenITriesToClaimAFileWithAInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = identity.Files.ClaimFileOwnership("InvalidFileId", request).Result;
    }

    [When($"{RegexFor.SINGLE_THING} tries to claim a non-existing file")]
    public void WhenITriesToClaimANonExistingFile(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = identity.Files.ClaimFileOwnership(FileId.New(), request).Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/RegenerateOwnershipToken endpoint")]
    public void WhenISendsAPatchRequestToTheFilesFIdRegenerateOwnershipTokenEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken(fileId).Result;

        if (_resetOwnershipTokenResponse.Result != null)
            _filesContext.FileNameToOwnershipToken[fileName] = _resetOwnershipTokenResponse.Result!.NewOwnershipToken ?? "";
    }

    [When($"{RegexFor.SINGLE_THING} tries to regenerate the OwnershipToken for an invalid FileId")]
    public void WhenITriesToRegenerateTheOwnershiptokenForAnInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken("InvalidFileId").Result;
    }

    [When($"{RegexFor.SINGLE_THING} tries to regenerate the OwnershipToken of a non-existing FileId")]
    public void WhenITriesToRegenerateTheOwnershipTokenOfANonExistingFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken(FileId.New()).Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a Post request to the /Files/{RegexFor.SINGLE_THING}.Id/ValidateOwnershipToken with the file's ownership token")]
    public async Task WhenISendsAPostRequestToTheFilesFIdValidateOwnershipTokenWithTheFilesOwnershipToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files[fileName].Id;
        var token = _filesContext.FileNameToOwnershipToken[fileName];
        var request = new ValidateFileOwnershipTokenRequest { FileOwnershipToken = token };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(fileId, request);
    }

    [When($"{RegexFor.SINGLE_THING} tries to validate an OwnershipToken of an invalid FileId")]
    public void WhenITriesToValidateTheOwnershipTokenOfAnInvalidFileId(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ValidateFileOwnershipTokenRequest { FileOwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = identity.Files.ValidateFileOwnershipToken("InvalidFileId", request).Result;
    }

    [When($"{RegexFor.SINGLE_THING} tries to validate the token using an valid FileId of a non-existing file")]
    public void WhenITriesToValidateTheTokenUsingAnValidFileIdOfANonExistingFile(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var request = new ValidateFileOwnershipTokenRequest { FileOwnershipToken = FileOwnershipToken.New().Value };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = identity.Files.ValidateFileOwnershipToken(FileId.New(), request).Result;
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

        _filesContext.FileNameToOwnershipToken[fileName].Should().BeEquivalentTo(responseToken);
    }


    [Then($"{RegexFor.SINGLE_THING} is the new owner of {RegexFor.SINGLE_THING}")]
    public async Task ThenIIsTheNewOwnerOfF(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        // The file needs to be reloaded to get the latest owner
        var file = _filesContext.Files[fileName];
        var reloadedFile = await identity.Files.GetFileMetadata(file.Id);
        _filesContext.Files[fileName] = reloadedFile.Result!;

        identity.IdentityData.Should().NotBeNull();
        identity.IdentityData!.Address.Should().NotBeNullOrEmpty();
        identity.IdentityData!.Address.Should().Be(_filesContext.Files[fileName].Owner);
    }

    [Then($"the ownership of {RegexFor.SINGLE_THING} is locked")]
    public void ThenTheOwnershipOfFIsLocked(string fileName)
    {
        var file = _filesContext.Files[fileName];
        file.Should().NotBeNull();
        file!.OwnershipIsLocked.Should().BeTrue();
    }

    [Then($"the ownership of {RegexFor.SINGLE_THING} is not locked")]
    public void ThenTheOwnershipOfFIsNotLocked(string fileName)
    {
        var file = _filesContext.Files[fileName];
        file.Should().NotBeNull();
        file!.OwnershipIsLocked.Should().BeFalse();
    }

    [Then($"the ValidateOwnershipTokenResponse contains {RegexFor.SINGLE_THING}")]
    public void ThenTheValidateOwnershipTokenResponseContainsExpected(string expected)
    {
        _validateOwnershipTokenResponse.Should().NotBeNull();
        _validateOwnershipTokenResponse.Result.Should().NotBeNull();
        _validateOwnershipTokenResponse.Result!.IsValid.Should().Be(expected == "true");
    }

    #endregion
}
